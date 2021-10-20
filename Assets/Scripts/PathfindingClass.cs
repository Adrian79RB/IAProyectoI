using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingClass
{
    public static float distanciaWaypoint = 0.5f;

    //Buscar nodo más cercano al NPC
    public static Transform encontrarNodoCercano(Transform npc)
    {
        bool encontrado = false;
        float minDistance = float.MaxValue;
        float sphereRadious = 1f;
        Transform currentNode = null;

        while (!encontrado)
        {
            Collider[] waypoints = Physics.OverlapSphere(npc.position, sphereRadious, LayerMask.GetMask("Waypoints"));
            foreach (Collider waypoint in waypoints)
            {
                float distance = Vector3.Distance(waypoint.transform.position, npc.position);
                if (waypoint.GetComponent<Nodo>() != null && distance < minDistance)
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

        return currentNode;
    }

    public static void obtenerCamino(Transform npc, Nodo target, Nodo start, ref int[] nodesList)
    {
        //Creamos el nodo inicial del pathfinding
        Nodo nodoActual = target;
        nodoActual.costSoFar = 0;
        float distanceToGhost = Vector3.Distance(nodoActual.transform.position, npc.position);
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
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    closedQueue.EliminarNodo(nodoAuxiliar);
                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoActual.costSoFar;
                }
                else if (openedQueue.EncontrarNodo(nextNode))
                {
                    Nodo nodoAuxiliar = openedQueue.ConsultarNodo(nextNode);
                    if (nodoAuxiliar != null && nodoAuxiliar.costSoFar <= distanceToNextNode)
                        continue;

                    distanceToGhost = nodoAuxiliar.estimatedTotalCost - nodoAuxiliar.costSoFar;
                }
                else
                {
                    distanceToGhost = Vector3.Distance(npc.position, nextNode.transform.position);
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

        if (nodoActual == start)
        {
            while (nodoActual != target)
            {
                Debug.Log("x Nodo Actual: " + nodoActual.name);
                Debug.Log("x Padre: " + nodoActual.father.name);
                nodesList[nodoActual.getId()] = nodoActual.father.getId();
                Nodo aux = nodoActual.father;
                //nodoActual.father = null;
                nodoActual = aux;
            }
        }
    }
}
