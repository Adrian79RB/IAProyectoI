using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CazadorMovement : MonoBehaviour
{
    public Transform player;
    public Transform homePoint;
    public Transform nearestNode;
    public int cazadorId;

    float velocidadCaza = 3.5f;
    float velocidadRotacion = 0.1f;
    float distanciaWaypoint = 0.5f;
    Vector3 velocidad;

    [SerializeField]int estado; //Alerted: Jugador coge monedas; GoingHome: Vuelta al inicio; Waiting: Espera en casa; SearchingPatrol: Busca al jugador; GoingPatrol: Persigue al jugador

    Transform WaypointFather; //Guardamos en cada indice correspondiente a un nodo, el id de su padre con el que forma el camino al objetivo
    Transform objetivoActual;
    int[] pathWaypoints;

    void Start()
    {
        velocidad = transform.forward;
        WaypointFather = GameObject.Find("PadreWaypoints").transform;
        objetivoActual = transform;
        pathWaypoints = new int[WaypointFather.childCount];
        estado = EstadoNPC.Waiting;
    }

    private void Update()
    {
        //Movimiento();
        Seek(objetivoActual.position);
        ControlDeEstados();
        ComprobarWaypoints();
    }

    /*void Movimiento()
    {
        if(objetivoActual != transform)
        {
            Vector3 direction = (objetivoActual.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, velocidadRotacion);
            transform.position += direction * velocidadCaza * Time.deltaTime;
        }
    }*/

    private void ControlDeEstados()
    {
        if (estado == EstadoNPC.Alerted)
        {
            if (nearestNode == null)
                nearestNode = PathfindingClass.encontrarNodoCercano(transform);

            PathfindingClass.obtenerCamino(transform, homePoint.GetComponent<Nodo>(), nearestNode.GetComponent<Nodo>(), ref pathWaypoints);
            cambiarEstadoCazador(EstadoNPC.GoingHome);
            objetivoActual = nearestNode;
        }
        else if (estado == EstadoNPC.SearchingPatrol || estado == EstadoNPC.GoingPatrol)
        {
            Transform nodoCazador = PathfindingClass.encontrarNodoCercano(transform);
            Transform objetivo = PathfindingClass.encontrarNodoCercano(player);
            Nodo nodoObjetivo = objetivo.GetComponent<Nodo>();
            if (nodoObjetivo != null)
            {
                if (cazadorId == 1 && nodoObjetivo.arcs.Count > cazadorId)
                    objetivo = nodoObjetivo.arcs[cazadorId].transform;
                else if (cazadorId == 2 && nodoObjetivo.arcs.Count > cazadorId)
                    objetivo = nodoObjetivo.arcs[cazadorId].transform;

                if (estado == EstadoNPC.SearchingPatrol)
                {
                    nearestNode = objetivo;
                    PathfindingClass.obtenerCamino(transform, nearestNode.GetComponent<Nodo>(), nodoCazador.GetComponent<Nodo>(), ref pathWaypoints);
                    cambiarEstadoCazador(EstadoNPC.GoingPatrol);
                    objetivoActual = nodoCazador;
                }
                else if (estado == EstadoNPC.GoingPatrol && objetivo != nearestNode)
                {
                    nearestNode = objetivo;
                    PathfindingClass.obtenerCamino(transform, nearestNode.GetComponent<Nodo>(), nodoCazador.GetComponent<Nodo>(), ref pathWaypoints);
                    cambiarEstadoCazador(EstadoNPC.GoingPatrol);
                }
            }
        }
    }

    void ComprobarWaypoints()
    {
        float dist = Vector3.Distance(objetivoActual.position, transform.position);
        if(dist < distanciaWaypoint)
        {
            if(estado == EstadoNPC.GoingHome)
            {
                if(objetivoActual != homePoint)
                {
                    int id = objetivoActual.GetComponent<Nodo>().getId();
                    objetivoActual = WaypointFather.GetChild(pathWaypoints[id]);
                }
                else
                {
                    cambiarEstadoCazador(EstadoNPC.Waiting);
                }
            }
            else if(estado == EstadoNPC.GoingPatrol)
            {
                if(objetivoActual != nearestNode)
                {
                    int id = objetivoActual.GetComponent<Nodo>().getId();
                    objetivoActual = WaypointFather.GetChild(pathWaypoints[id]);
                }
                else
                {
                    objetivoActual = player;
                }
            }
            else if(estado == EstadoNPC.Waiting)
            {
                objetivoActual = transform;
            }
        }
    }

    // ---------------------
    //Steering Behaviour
    //----------------------
    void Seek(Vector3 target)
    {
        if (target != transform.position)
        {

            Vector3 distanciaObjetivo = target - transform.position;
            Vector3 velocidadDeseada = distanciaObjetivo.normalized * velocidadCaza;
            Vector3 steer = velocidadDeseada - velocidad;

            velocidad += steer * Time.deltaTime;

            float factorFrenado = Mathf.Clamp01(distanciaObjetivo.magnitude / distanciaWaypoint);
            velocidad *= factorFrenado;

            transform.position += velocidad * Time.deltaTime;

            npcRotation(distanciaObjetivo.normalized);
        }
    }

    void npcRotation(Vector3 distanciaObjetivo)
    {
        Quaternion rotacion = Quaternion.LookRotation(distanciaObjetivo, transform.up);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);
    }

    void cambiarEstadoCazador(int nuevoEstado)
    {
        estado = nuevoEstado;
    }

    //--------------------------------------
    //Comunicación entre NPCs
    //--------------------------------------

    void AvisarFantasma(GameObject fantasma)
    {
        fantasma.GetComponent<MovimientoFantasmas>().AvisoDeCazador();
    }

    public void AvisoDeFantasma(){
        cambiarEstadoCazador(EstadoNPC.SearchingPatrol);
    }

    public void AvisoDeMonedas()
    {
        cambiarEstadoCazador(EstadoNPC.Alerted);
    }

    public void AvisoDeGargola()
    {
        cambiarEstadoCazador(EstadoNPC.SearchingPatrol);
    }
}
