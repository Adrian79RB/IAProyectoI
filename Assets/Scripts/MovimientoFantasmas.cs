using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public int estado;
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

    Queue<Transform> pathQueue;
    bool pathFound = false;

    void Start()
    {
        objetivoActual = Waypoints[waypointObjetivo];
        pathQueue = new Queue<Transform>();
        estado = 
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ComprobarWaypoint();
        if (alertado)
        {
            if (!pathFound)
            {
                encontrarNodoCercano();
                obtenerCaminoACasa(homePoint.GetComponent<Nodo>());
            }
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

    void obtenerCaminoACasa(Nodo target)
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

            if (nodoActual.transform == nearestNode)
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

        if (nodoActual.transform != nearestNode)
        {
            pathFound = false;
            pathQueue.Clear();
        }
        else
        {
            Debug.Log("Haciendo el camino de padres.");
            while(nodoActual.transform != target.transform)
            {
                Debug.Log("Metemos en pathQueue el nodo: " + nodoActual.transform.name);
                pathQueue.Enqueue(nodoActual.transform);
                nodoActual = nodoActual.father;
            }

            pathQueue.Enqueue(nodoActual.transform);
            pathFound = true;
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
        cambiarEstadoFantasma(true);
        pathFound = false;
    }

    public void AvisoDeCazador()
    {
        cambiarEstadoFantasma(false);
        pathFound = false;

    }

    

}
