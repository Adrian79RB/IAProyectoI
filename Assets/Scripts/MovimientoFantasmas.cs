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
    float velocidadMovimiento = 5f;
    float velocidadActual = 0.0f;
    
    //Velocidad de rotacioon
    float velocidadRotacion = 0.15f;
    
    //Valores varios
    float distanciaWaypoint = 0.5f;
    float sphereRadious = 1f;
    float ghostCallRadious = 25f;
    [SerializeField]int estado; //Patrolling: Haciendo su patrulla; Alerted: Jugador es detectado; GoingHome: Vuelta al inicio; Waiting: Espera en casa; SearchingPatrol: Busca lugar patrulla; GoingPatrol: Va hasta la zona patrulla

    //Steering Behaviour
    Vector3 initialPoint;
    Vector3 endPoint;
    float pathRadious = 10;

    //Path Finding
    Transform WaypointFather;
    Transform objetivoActual;
    Rigidbody rgbd;
    int[] pathWaypoints;
    int[] patrolWaypoints;

    void Start()
    {
        rgbd = GetComponent<Rigidbody>();
        WaypointFather = GameObject.Find("PadreWaypoints").transform;
        objetivoActual = patrolStart;
        initialPoint = objetivoActual.position;
        pathWaypoints = new int[WaypointFather.childCount];
        patrolWaypoints = new int[WaypointFather.childCount];
        estado = EstadoNPC.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        Follow();
        ControlDeEstados();
        ComprobarWaypoint();
    }

    void Movimiento(){
        if(objetivoActual != transform)
        {
            //Vector3 direccion = (objetivo - this.transform.position).normalized;
            Vector3 direccion = (objetivoActual.position - this.transform.position).normalized;
            Quaternion rotacion = Quaternion.LookRotation(direccion, transform.up);
            rgbd.MoveRotation(Quaternion.Lerp(this.transform.rotation, rotacion, velocidadRotacion));

            if (estado == EstadoNPC.Patrolling)
                velocidadActual = velocidadPatrol;
            else
                velocidadActual = velocidadMovimiento;

            rgbd.MovePosition(this.transform.position + direccion * velocidadActual * Time.deltaTime);
        }
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
            initialPoint = objetivoActual.position;

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

            endPoint = objetivoActual.position;
        }
    }
    // ---------------------
    //Steering Behaviour
    //----------------------
    void Seek(Vector3 target)
    {
        Vector3 deseado = target - transform.position;
        deseado = deseado.normalized * velocidadActual;
        Vector3 steer = deseado - rgbd.velocity;
        rgbd.AddForce(steer, ForceMode.Acceleration);
    }

    void Follow()
    {
        Vector3 prediccion = rgbd.velocity;
        prediccion = transform.position + prediccion.normalized * 50;
        //Vector3 predicPos = transform.position + prediccion;

        Vector3 puntoNormal = PathfindingClass.getNormalPoint(initialPoint, endPoint, prediccion);
        Vector3 dir = endPoint - initialPoint;
        dir = dir.normalized * rgbd.velocity.magnitude;
        Vector3 target = puntoNormal + dir;

        float distanceToPath = Vector3.Distance(puntoNormal, prediccion);
        if (distanceToPath > pathRadious)
        {
            Seek(target);
            //Movimiento(target);
        }

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
    //Comunicación entre NPCs
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
