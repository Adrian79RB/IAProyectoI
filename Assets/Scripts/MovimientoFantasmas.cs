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

    float velocidad = 2f;
    float velocidadRotacion = 0.15f;
    float distanciaWaypoint = 0.5f;
    float sphereRadious = 1f;
    [SerializeField]int estado;

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
        if(estado == EstadoNPC.Patrolling)
        {
            float dist = Vector3.Distance(objetivoActual.position, transform.position);
            if (objetivoActual == patrolEnd && dist < distanciaWaypoint)
            {
                obtenerCamino(patrolStart.GetComponent<Nodo>(), patrolEnd.GetComponent<Nodo>(), patrolWaypoints);
            }
            else if (objetivoActual == patrolStart && dist < distanciaWaypoint)
            {
                obtenerCamino(patrolEnd.GetComponent<Nodo>(), patrolStart.GetComponent<Nodo>(), patrolWaypoints);
            }
        }
        else if (estado == EstadoNPC.Alerted)
        {
            if(nearestNode == null)
                encontrarNodoCercano();

            obtenerCamino(homePoint.GetComponent<Nodo>(), nearestNode.GetComponent<Nodo>(), pathWaypoints);
            cambiarEstadoFantasma(EstadoNPC.GoingHome);
            objetivoActual = nearestNode;
        }
        else if(estado == EstadoNPC.SearchingPatrol)
        {
            obtenerCamino(nearestNode.GetComponent<Nodo>(), homePoint.GetComponent<Nodo>(), pathWaypoints);
            cambiarEstadoFantasma(EstadoNPC.GoingPatrol);
            objetivoActual = homePoint;
        }
        ComprobarWaypoint();
    }

    void Movimiento(){
        Vector3 direccion = (objetivoActual.position - this.transform.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion, transform.up);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);
        this.transform.position = this.transform.position + direccion * velocidad * Time.deltaTime;
    }
    void ComprobarWaypoint()
    {
        float dist = Vector3.Distance(objetivoActual.position, this.transform.position);
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

    void obtenerCamino(Nodo target, Nodo start, int[] nodesList)
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
            Debug.Log("Tama�o cola abierta antes de Devolver: " + openedQueue.getLegth());
            nodoActual = openedQueue.Devolver();
            Debug.Log("Tama�o cola abierta despues de Devolver: " + openedQueue.getLegth());
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
        }

        if (nodoActual.transform == start.transform)
        {
            Debug.Log("Haciendo el camino de padres.");
            while(nodoActual != target)
            {
                Debug.Log("Metemos en pathQueue el nodo: " + nodoActual.transform.name);
                nodesList[nodoActual.getId()] = nodoActual.father.getId();
                Nodo aux = nodoActual.father;
                nodoActual.father = null;
                nodoActual = aux;
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
        cambiarEstadoFantasma(EstadoNPC.SearchingPatrol);
    }

    

}
