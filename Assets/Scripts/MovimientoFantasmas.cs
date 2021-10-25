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

    //Velocidades de moviemiento
    float velocidadPatrol = 2f;
    float velocidadMovimiento = 3.5f;
    float velocidadActual = 0.0f;
    
    //Velocidad de rotacioon
    float velocidadRotacion = 0.15f;
    
    //Valores varios
    float distanciaWaypoint = 1.5f;
    float ghostCallRadious = 25f;
    float cazadorCallRadious = 3.5f;
    
    [SerializeField]int estado; //Patrolling: Haciendo su patrulla; Alerted: Jugador es detectado; GoingHome: Vuelta al inicio; Waiting: Espera en casa; SearchingPatrol: Busca lugar patrulla; GoingPatrol: Va hasta la zona patrulla

    //Steering Behaviour
    //Vector3 initialPoint;
    //Vector3 endPoint;
    Vector3 velocidad;
    //float pathRadious = 1.5f;

    //Path Finding
    Transform WaypointFather;
    Transform objetivoActual;
    int[] pathWaypoints;
    int[] patrolWaypoints;

    void Start()
    {
        //Path Following
        objetivoActual = patrolStart;
        //initialPoint = transform.position;
        //endPoint = patrolStart.position;
        velocidad = transform.forward;

        //Path Finding
        WaypointFather = GameObject.Find("PadreWaypoints").transform;
        pathWaypoints = new int[WaypointFather.childCount];
        patrolWaypoints = new int[WaypointFather.childCount];
        estado = EstadoNPC.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        //Movimiento();
        //Follow();
        Seek(objetivoActual.position);
        ControlDeEstados();
        ComprobarWaypoint();
    }

   /*void Movimiento(){
        if(objetivoActual != transform)
        {
            //Vector3 direccion = (objetivo - this.transform.position).normalized;
            Vector3 direccion = (objetivoActual.position - this.transform.position).normalized;
            Quaternion rotacion = Quaternion.LookRotation(direccion, transform.up);
            transform.rotation= Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);

            if (estado == EstadoNPC.Patrolling)
                velocidadActual = velocidadPatrol;
            else
                velocidadActual = velocidadMovimiento;

            transform.position = this.transform.position + direccion * velocidadActual * Time.deltaTime;
        }
    }*/

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
            LanzarAvisoCazador();
        }
        else if (estado == EstadoNPC.SearchingPatrol)
        {
            nearestNode = PathfindingClass.encontrarNodoCercano(transform);

            PathfindingClass.obtenerCamino(transform, patrolStart.GetComponent<Nodo>(), nearestNode.GetComponent<Nodo>(), ref pathWaypoints);
            cambiarEstadoFantasma(EstadoNPC.GoingPatrol);
            objetivoActual = nearestNode;
        }
    }

    void ComprobarWaypoint()
    {
        float dist = Vector3.Distance(objetivoActual.position, transform.position);
        if (dist < distanciaWaypoint)
        {
            //initialPoint = objetivoActual.position;

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
                if (objetivoActual != patrolStart)
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

            //endPoint = objetivoActual.position;
        }
    }
    // ---------------------
    //Steering Behaviour
    //----------------------
    void Seek(Vector3 target)
    {
        if(target != transform.position)
        {
            if (estado == EstadoNPC.Patrolling)
                velocidadActual = velocidadPatrol;
            else
                velocidadActual = velocidadMovimiento;

            Vector3 distanciaObjetivo = target - transform.position;
            Vector3 velocidadDeseada = distanciaObjetivo.normalized * velocidadActual;
            Vector3 steer = velocidadDeseada - velocidad;

            velocidad += steer * Time.deltaTime;

            float factorFrenado = Mathf.Clamp01(distanciaObjetivo.magnitude / distanciaWaypoint);
            velocidad *= factorFrenado;

            transform.position += velocidad * Time.deltaTime;

            npcRotation(distanciaObjetivo.normalized);
        }
    }

    /*void Follow()
    {
        if (estado == EstadoNPC.Patrolling)
            velocidadActual = velocidadPatrol;
        else
            velocidadActual = velocidadMovimiento;

        Vector3 prediccion = velocidad.normalized * 5;
        Vector3 predicPos = transform.position + prediccion;

        Vector3 puntoNormal = PathfindingClass.getNormalPoint(initialPoint, endPoint, predicPos);

        Vector3 dir = endPoint - initialPoint;
        Vector3 posFuturaSobreDir = dir.normalized * velocidadActual;
        Vector3 target = puntoNormal + posFuturaSobreDir;

        float distanceToPath = Vector3.Distance(puntoNormal, predicPos);

        if (distanceToPath > pathRadious)
        {
            Seek(target);
        }
        else
        {
            Seek(objetivoActual.position);
        }

    }*/

    void npcRotation(Vector3 distanciaObjetivo)
    {
        Quaternion rotacion = Quaternion.LookRotation(distanciaObjetivo, transform.up);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion);
    }

    //----------------------
    // Control de estado
    //----------------------
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
                if(fantasma.consultaEstadoFantasma() == EstadoNPC.GoingPatrol || fantasma.consultaEstadoFantasma() == EstadoNPC.Patrolling || fantasma.consultaEstadoFantasma() == EstadoNPC.SearchingPatrol)
                    fantasma.AvisoDeFantasma();
            }
        }
    }

    void LanzarAvisoCazador()
    {
        Collider[] npcs = Physics.OverlapSphere(transform.position, cazadorCallRadious);

        foreach( Collider npc in npcs)
        {
            if(npc.tag == "cazador")
            {
                CazadorMovement cazador = npc.GetComponent<CazadorMovement>();
                if(cazador.consultaEstadoCazador() == EstadoNPC.Waiting)
                    cazador.AvisoDeFantasma();
            }
        }
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

    public void AvisoDeMonedas()
    {
        cambiarEstadoFantasma(EstadoNPC.SearchingPatrol);
    }
}
