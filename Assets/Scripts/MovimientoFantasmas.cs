using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public bool alertado = false;
    public Transform ultimaPosicionConocidaJugador;
    public Transform objetivoActual;
    public Transform homePoint;
    public Transform nearestNode;
    public Transform[] Waypoints;

    float velocidad = 2f;
    float velocidadRotacion = 0.15f;
    int waypointObjetivo = 0;
    float distanciaWaypoint = 0.2f;
    float sphereRadious = 1f;

    Queue<Transform> pathQueue;
    bool ghostFound = false;

    void Start()
    {
        objetivoActual = Waypoints[waypointObjetivo];
        pathQueue = new Queue<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ComprobarWaypoint();
        if (alertado)
        {
            if (!ghostFound)
            {
                encontrarNodoCercano();
                obtenerCaminoACasa();
            }
        }
    }

    void Movimiento(){
        Vector3 direccion = (objetivoActual.transform.position-this.transform.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion,transform.up);
        this.transform.position = this.transform.position + direccion * velocidad * Time.deltaTime;
        transform.rotation = Quaternion.Lerp( this.transform.rotation, rotacion, velocidadRotacion);
    }
    void ComprobarWaypoint(){
        float dist = Vector3.Distance(objetivoActual.position, this.transform.position);
        if(dist < distanciaWaypoint){
            if (!alertado)
            {
                if (waypointObjetivo + 1 == Waypoints.Length)
                {
                    waypointObjetivo = 0;
                }
                else
                {
                    waypointObjetivo += 1;
                }
                objetivoActual = Waypoints[waypointObjetivo];
            }
            else 
            {
                if (pathQueue.Count > 0)
                {
                    objetivoActual = pathQueue.Dequeue();
                }
                else
                    objetivoActual = transform;
            }
        }
    }

    void encontrarNodoCercano() 
    {
        bool encontrado = false;
        float minDistance = float.MaxValue;
        Transform currentNode = null;
        
        while (!encontrado) 
        {
            Collider[] waypoints = Physics.OverlapSphere(transform.position, sphereRadious, LayerMask.GetMask("Waypoints"));
            foreach( Collider waypoint in waypoints)
            {
                float distance = Vector3.Distance(waypoint.transform.position, transform.position);
                if (waypoint != null &&  distance < minDistance)
                {
                    minDistance = distance;
                    currentNode = waypoint.transform;
                }
            }

            if (currentNode == null)
                sphereRadious++;
            else
            {
                encontrado = true;
                sphereRadious = 1f;
            }
        }

        nearestNode = currentNode;
    }

    void obtenerCaminoACasa()
    {
        //Creamos el nodo inicial del pathfinding
        Nodo nodoActual = homePoint.GetComponent<Nodo>();
        nodoActual.costSoFar = 0;
        float distanceToGhost = Vector3.Distance(nodoActual.transform.position, transform.position);
        nodoActual.estimatedTotalCost = nodoActual.costSoFar + distanceToGhost;

        //Creamos las listas abierta y cerrada
        priorityQueue openedQueue = new priorityQueue();
        priorityQueue closedQueue = new priorityQueue();

        openedQueue.Insertar(nodoActual, nodoActual.estimatedTotalCost);

        while (openedQueue.getLegth() > 0)
        {
            nodoActual = openedQueue.Devolver();

            if (nodoActual.transform == nearestNode)
                break;

            for (int i = 0; i < nodoActual.arcs.Count; i++)
            {
                Nodo nextNode = nodoActual.arcs[i].GetComponent<Nodo>();
                nextNode.costSoFar = nodoActual.costSoFar + nodoActual.weigths[i];

                if (closedQueue.EncontrarNodo(nextNode))
                {
                    Nodo nodoAuxiliar = closedQueue.ConsultarNodo(nextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= nextNode.costSoFar)
                        continue;

                    closedQueue.EliminarNodo(nodoAuxiliar);
                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoActual.costSoFar;
                    nextNode.father = nodoActual;
                }
                else if (openedQueue.EncontrarNodo(nextNode))
                {
                    Nodo nodoAuxiliar = openedQueue.ConsultarNodo(nextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= nextNode.costSoFar)
                        continue;

                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoAuxiliar.costSoFar;
                    nextNode.father = nodoActual;
                }
                else
                {
                    distanceToGhost = nextNode.costSoFar + Vector3.Distance(transform.position, nextNode.transform.position);
                    nextNode.father = nodoActual;
                }

                if (openedQueue.EncontrarNodo(nextNode))
                    openedQueue.CambiarPrio(nextNode, nextNode.costSoFar + distanceToGhost);
                else if (closedQueue.EncontrarNodo(nextNode))
                    closedQueue.CambiarPrio(nextNode, nextNode.costSoFar + distanceToGhost);
                else
                    openedQueue.Insertar(nextNode, nextNode.costSoFar + distanceToGhost);

                closedQueue.Insertar(nodoActual, nodoActual.estimatedTotalCost);

            }
        }

        if (nodoActual.transform != nearestNode)
        {
            ghostFound = false;
            pathQueue.Clear();
        }
        else
        {
            while(nodoActual.transform == homePoint)
            {
                pathQueue.Enqueue(nodoActual.transform);
                nodoActual = nodoActual.father;
            }

            ghostFound = true;
        }
    }

    public void cambiarEstadoFantasma(bool estado)
    {
        alertado = estado;
    }

    public bool consultaEstadoFantasma()
    {
        return alertado;
    }

    void AvisarCazador(GameObject cazador){
       // Movimiento(objetivoActual = cazador);
       cazador.GetComponent<CazadorMovement>().AvisoDeFantasma();
    }

    void OnTriggerEnter(Collider col){
         if (col.gameObject.tag == "cazador"){
             AvisarCazador(col.gameObject);
         }
    }

    public void AvisoDeGargola(){
        alertado = true;
    }

    

}
