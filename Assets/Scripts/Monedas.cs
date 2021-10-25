using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monedas : MonoBehaviour
{
    [SerializeField]
    private ActivarSalida activarSalida;
    private GameObject fantasmaPadre;
    private GameObject cazador;

    void Start()
    {
        activarSalida = FindObjectOfType<ActivarSalida>();
        fantasmaPadre = GameObject.Find("Fantasmas");
        cazador = GameObject.Find("cazadorIA");
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Contador = " + activarSalida.contadorMonedas);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            this.gameObject.SetActive(false);

            activarSalida.contadorMonedas++;
            activarSalida.textChange = true;

            LanzarAvisoMonedas();
        }
    }

    void LanzarAvisoMonedas(){
        if (cazador.GetComponent<CazadorMovement>().consultaEstadoCazador() == EstadoNPC.GoingPatrol)
        {
            cazador.GetComponent<CazadorMovement>().AvisoDeMonedas();
        }

        for (int i = 0; i < fantasmaPadre.transform.childCount; i++)
        {
            MovimientoFantasmas fantasma = fantasmaPadre.transform.GetChild(i).GetComponent<MovimientoFantasmas>();
            if (fantasma.consultaEstadoFantasma() != EstadoNPC.Patrolling)
                fantasma.AvisoDeMonedas();
        }
    }
}
