using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform ultimaPosicionConocidaJugador;
    public Transform objetivoActual;
    public Transform homePoint;
    public Transform nearestNode;
    public Transform[] Waypoints;

    float velocidad = 2f;
    float velocidadRotacion = 0.15f;
    int waypointObjetivo = 0;
    float distanciaWaypoint = 0.5f;
    float sphereRadious = 1f;
    [SerializeField]int estado;

    Queue<Transform> pathQueue;

    void Start()
    {
        objetivoActual = Waypoints[waypointObjetivo];
        pathQueue = new Queue<Transform>();
        estado = EstadoNPC.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ComprobarWaypoint();
        if (estado == EstadoNPC.Alerted)
        {
            if(nearestNode == null)
                encontrarNodoCercano();

            obtenerCamino(homePoint.GetComponent<Nodo>(), nearestNode.GetComponent<Nodo>());
            cambiarEstadoFantasma(EstadoNPC.GoingHome);
        }
        else if(estado == EstadoNPC.SearchingPatrol)
        {
            obtenerCamino(nearestNode.GetComponent<Nodo>(), homePoint.GetComponent<Nodo>());
            cambiarEstadoFantasma(EstadoNPC.GoingPatrol);
        }
    }

    void Movimiento(){
        Vector3 direccion = (objetivoActual.position - this.transform.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion, transform.up);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);
        this.transform.position = this.transform.position + direccion * velocidad * Time.deltaTime;
    }
    void ComprobarWaypoint(){
        float dist = Vector3.Distance(objetivoActual.position, this.transform.position);
        if(dist < distanciaWaypoint){
            if (estado == EstadoNPC.Patrolling)
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
            else if(estado == EstadoNPC.GoingHome)
            {
                if (pathQueue.Count > 0)
                {
                    objetivoActual = pathQueue.Dequeue();
                }
                else
                {
                    cambiarEstadoFantasma(EstadoNPC.Waiting);
                }
            }
            else if(estado == EstadoNPC.GoingPatrol)
            {
                if (pathQueue.Count > 0)
                {
                    objetivoActual = pathQueue.Dequeue();
                }
                else
                {
                    cambiarEstadoFantasma(EstadoNPC.Patrolling);
                }
            }
            else if(estado == EstadoNPC.Waiting)
            {
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

    void obtenerCamino(Nodo target, Nodo start)
    {
        //Creamos el nodo inicial del pathfinding
        Nodo nodoActual = target;
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
            Debug.Log("Nodo actual: " + nodoActual.transform.name);

            if (nodoActual == start)
                break;

            for (int i = 0; i < nodoActual.arcs.Count; i++)
            {
                Nodo nextNode = nodoActual.arcs[i].GetComponent<Nodo>();
                float distanceToNextNode = nodoActual.costSoFar + nodoActual.weigths[i];

                if (closedQueue.EncontrarNodo(nextNode))
                {
                    Nodo nodoAuxiliar = closedQueue.ConsultarNodo(nextNode);
                    Debug.Log("Nodo Auxiliar: " + nodoAuxiliar.transform.name + "; CostSoFar: " + nodoAuxiliar.costSoFar + "; Coste Estimado: " + nodoAuxiliar.estimatedTotalCost);
                    Debug.Log("Nodo Siguiente: " + nextNode.transform.name + "; CostSoFar: " + distanceToNextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    closedQueue.EliminarNodo(nodoAuxiliar);
                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoActual.costSoFar;
                }
                else if (openedQueue.EncontrarNodo(nextNode))
                {
                    Nodo nodoAuxiliar = openedQueue.ConsultarNodo(nextNode);
                    Debug.Log("Nodo Auxiliar: " + nodoAuxiliar.transform.name + "; CostSoFar: " + nodoAuxiliar.costSoFar + "; Coste Estimado: " + nodoAuxiliar.estimatedTotalCost);
                    Debug.Log("Nodo Siguiente: " + nextNode.transform.name + "; CostSoFar: " + distanceToNextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoAuxiliar.costSoFar;
                }
                else
                {
                    distanceToGhost = Vector3.Distance(transform.position, nextNode.transform.position);
                }

                nextNode.costSoFar = distanceToNextNode;
                nextNode.estimatedTotalCost = distanceToNextNode + distanceToGhost;
                nextNode.father = nodoActual;

                if (!openedQueue.EncontrarNodo(nextNode) && !closedQueue.EncontrarNodo(nextNode))
                    openedQueue.Insertar(nextNode, nextNode.estimatedTotalCost);
                else
                    openedQueue.CambiarPrio(nextNode, nextNode.estimatedTotalCost);

            }
            closedQueue.Insertar(nodoActual, nodoActual.estimatedTotalCost);
            Debug.Log("Cola cerrada");
            closedQueue.MostrarContenido();
            Debug.Log("Cola abierta");
            openedQueue.MostrarContenido();
        }

        if (nodoActual.transform != start.transform)
        {
            pathQueue.Clear();
        }
        else
        {
            Debug.Log("Haciendo el camino de padres.");
            while(nodoActual != target)
            {
                Debug.Log("Metemos en pathQueue el nodo: " + nodoActual.transform.name);
                pathQueue.Enqueue(nodoActual.transform);
                nodoActual = nodoActual.father;
            }

            pathQueue.Enqueue(nodoActual.transform);
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
        cambiarEstadoFantasma(EstadoNPC.Alerted);
    }

    public void AvisoDeCazador()
    {
        cambiarEstadoFantasma(EstadoNPC.GoingPatrol);
    }

    

}
