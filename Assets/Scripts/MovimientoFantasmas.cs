using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public bool alertado = false;
    public Transform ultimaPosicionConocidaJugador;
    public float velocidad = 2f;
    public float velocidadRotacion = 0.15f;
    public int waypointObjetivo = 0;
    public float distanciaWaypoint = 0.2f;
    public Transform objetivoActual;
    public Transform homePoint;
    public Transform[] Waypoints;

    Stack<Transform> pathStack;
    bool ghostFound = false;

    void Start()
    {
        objetivoActual = Waypoints[waypointObjetivo];
        pathStack = new Stack<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ComprobarWaypoint();
        if (alertado)
        {
            if(!ghostFound)
                obtenerCaminoACasa();
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
                if (pathStack.Count > 0)
                {
                    objetivoActual = pathStack.Peek();
                    pathStack.Pop();
                }
                else
                    objetivoActual = transform;
            }
        }
    }

    void obtenerCaminoACasa()
    {
        Nodo nodoActual = homePoint.GetComponent<Nodo>();
        float distanceToGhost = Vector3.Distance(homePoint.position, transform.position);
        float distance = 0f;
        pathStack.Push(homePoint);
        while (!ghostFound)
        {
            for (int i = 0; i < nodoActual.arcs.Count; i++)
            {
                if (Vector3.Distance(nodoActual.arcs[i].transform.position, transform.position) < distanceToGhost)
                {
                    distance += nodoActual.weigths[i];
                    nodoActual = nodoActual.arcs[i].transform.GetComponent<Nodo>();
                    distanceToGhost = Vector3.Distance(nodoActual.transform.position, transform.position);
                    pathStack.Push(nodoActual.transform);
                    break;
                }
            }

            for (int i = 0; i < nodoActual.weigths.Count; i++) {
                if (distanceToGhost < nodoActual.weigths[i])
                    ghostFound = true;
            }
        }

        foreach (Transform waypoint in pathStack)
            Debug.Log("Waypoints camino casa: " + waypoint);
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
