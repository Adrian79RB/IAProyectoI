using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoilObserver : MonoBehaviour
{
    public Transform player;
    public Transform parent;
    public GameEnding gameEnding;
    private Quaternion basePosition;
    bool m_IsPlayerInRange;
    float gargoilCallRadious = 20f;

    //cuando algo entra en su campo de visión comprueba si se trata del jugador
    void Start(){
        basePosition = parent.rotation;
    }
    void OnTriggerEnter (Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }
    //cuando algo sale del campo de visión deja de ver
    void OnTriggerExit (Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
            parent.rotation=basePosition;
        }
    }

    void Update ()
    {
        //si el jugador está en el campo de visión
        if (m_IsPlayerInRange)
        {
            //manda un raycast para confirmar que el jugador está en el campo de visión
            Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            //si confirma que está en el campo de visión hace cosas
            if (Physics.Raycast (ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    parent.LookAt(player);
                    LanzarAvisoFantasmas();
                    LanzarAvisoCazadores();
                    //gameEnding.CaughtPlayer ();
                }
            }
        }
    }

    void LanzarAvisoFantasmas(){
        Collider[] npcs = Physics.OverlapSphere(transform.position, gargoilCallRadious);

        foreach( Collider npc in npcs)
        {
            if(npc.tag == "fantasma")
            {
                MovimientoFantasmas fantasma = npc.GetComponent<MovimientoFantasmas>();
                if(fantasma.consultaEstadoFantasma() != EstadoNPC.Alerted && fantasma.consultaEstadoFantasma() != EstadoNPC.GoingHome && fantasma.consultaEstadoFantasma() != EstadoNPC.Waiting)
                    fantasma.AvisoDeGargola();
            }
        }
    }

    void LanzarAvisoCazadores()
    {
        Collider[] npcs = Physics.OverlapSphere(transform.position, gargoilCallRadious);

        foreach (Collider npc in npcs)
        {
            if (npc.tag == "cazador")
            {
                CazadorMovement cazador = npc.GetComponent<CazadorMovement>();
                if (cazador.consultaEstadoCazador() == EstadoNPC.GoingHome)
                    cazador.AvisoDeGargola();
            }
        }
    }
}

