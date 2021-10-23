using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform ultimaPosicionConocidaJugador;
    public Transform patrolStart;
    public Transform patrolEnd;
    public Transform homePoint;
    public Transform nearestNode;

    float velocidadPatrol = 2f;
    float velocidadMovimiento = 5f;
    float velocidadRotacion = 0.15f;
    float distanciaWaypoint = 0.5f;
    float sphereRadious = 1f;
    float ghostCallRadious = 25f;
    
    [SerializeField]int estado; //Patrolling: Haciendo su patrulla; Alerted: Jugador es detectado; GoingHome: Vuelta al inicio; Waiting: Espera en casa; SearchingPatrol: Busca lugar patrulla; GoingPatrol: Va hasta la zona patrulla

    Transform WaypointFather;
    Transform objetivoActual;
    int[] pathWaypoints;
    int[] patrolWaypoints;

    void Start()
    {
        WaypointFather = GameObject.Find("PadreWaypoints").transform;
        objetivoActual = patrolStart;
        pathWaypoints = new int[WaypointFather.childCount];
        patrolWaypoints = new int[WaypointFather.childCount];
        estado = EstadoNPC.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ControlDeEstados();
        ComprobarWaypoint();
    }

    void Movimiento(){
        Vector3 direccion = (objetivoActual.position - this.transform.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion, transform.up);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);

        if(estado == EstadoNPC.Patrolling)
            this.transform.position = this.transform.position + direccion * velocidadPatrol * Time.deltaTime;
        else
            this.transform.position = this.transform.position + direccion * velocidadMovimiento * Time.deltaTime;
    }

    void ControlDeEstados()
    {
        if (estado == EstadoNPC.Patrolling)
        {
            float dist = Vector3.Distance(objetivoActual.position, transform.position);
            if (objetivoActual == patrolEnd && dist < distanciaWaypoint)
            {
                PathfindingClass.obtenerCamino(transform, patrolStart.GetComponent<Nodo>(), patrolEnd.GetComponent<Nodo>(), ref patrolWaypoints);
            }
            else if (objetivoActual == patrolStart && dist < distanciaWaypoint)
            {
                PathfindingClass.obtenerCamino(transform, patrolEnd.GetComponent<Nodo>(), patrolStart.GetComponent<Nodo>(), ref patrolWaypoints);
            }
        }
        else if (estado == EstadoNPC.Alerted)
        {
            nearestNode = PathfindingClass.encontrarNodoCercano(transform);

            PathfindingClass.obtenerCamino(transform, homePoint.GetComponent<Nodo>(), nearestNode.GetComponent<Nodo>(), ref pathWaypoints);
            cambiarEstadoFantasma(EstadoNPC.GoingHome);
            objetivoActual = nearestNode;
        }
        else if (estado == EstadoNPC.GoingHome)
        {
            LanzarAvisoFantasma();
        }
        else if (estado == EstadoNPC.SearchingPatrol)
        {
            PathfindingClass.obtenerCamino(transform, nearestNode.GetComponent<Nodo>(), homePoint.GetComponent<Nodo>(), ref pathWaypoints);
            cambiarEstadoFantasma(EstadoNPC.GoingPatrol);
            objetivoActual = homePoint;
        }
    }

    void ComprobarWaypoint()
    {
        float dist = Vector3.Distance(objetivoActual.position, transform.position);
        if (dist < distanciaWaypoint)
        {
            if (estado == EstadoNPC.Patrolling)
            {
                int id = objetivoActual.GetComponent<Nodo>().getId();
                objetivoActual = WaypointFather.GetChild(patrolWaypoints[id]);
            }
            else if (estado == EstadoNPC.GoingHome)
            {
                if (objetivoActual != homePoint)
                {
                    int id = objetivoActual.GetComponent<Nodo>().getId();
                    objetivoActual = WaypointFather.GetChild(pathWaypoints[id]);
                }
                else
                {
                    cambiarEstadoFantasma(EstadoNPC.Waiting);
                }
            }
            else if (estado == EstadoNPC.GoingPatrol)
            {
                if (objetivoActual != nearestNode)
                {
                    int id = objetivoActual.GetComponent<Nodo>().getId();
                    objetivoActual = WaypointFather.GetChild(pathWaypoints[id]);
                }
                else
                {
                    cambiarEstadoFantasma(EstadoNPC.Patrolling);
                }
            }
            else if (estado == EstadoNPC.Waiting)
            {
                objetivoActual = transform;
            }
        }
    }

    public void cambiarEstadoFantasma(int newEstado)
    {
        estado = newEstado;
    }

    public int consultaEstadoFantasma()
    {
        return estado;
    }

    //------------------------------------
    //Comunicaci�n entre NPCs
    //------------------------------------

    void LanzarAvisoFantasma()
    {
        Collider[] npcs = Physics.OverlapSphere(transform.position, ghostCallRadious);

        foreach( Collider npc in npcs)
        {
            if(npc.tag == "fantasma")
            {
                MovimientoFantasmas fantasma = npc.GetComponent<MovimientoFantasmas>();
                if(fantasma.consultaEstadoFantasma() != EstadoNPC.Alerted && fantasma.consultaEstadoFantasma() != EstadoNPC.GoingHome && fantasma.consultaEstadoFantasma() != EstadoNPC.Waiting)
                    fantasma.AvisoDeFantasma();
            }
        }
    }

    void AvisarCazador(GameObject cazador){
       // Movimiento(objetivoActual = cazador);
       cazador.GetComponent<CazadorMovement>().AvisoDeFantasma();
    }

    public void AvisoDeGargola(){
        cambiarEstadoFantasma(EstadoNPC.Alerted);
    }
    public void AvisoDeFantasma()
    {
        cambiarEstadoFantasma(EstadoNPC.Alerted);
    }

    public void AvisoDeCazador()
    {
        cambiarEstadoFantasma(EstadoNPC.SearchingPatrol);
    }
}
