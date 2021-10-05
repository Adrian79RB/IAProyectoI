using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoFantasmas : MonoBehaviour
{
    // Start is called before the first frame update
    public bool alertado = false;
    public Transform ultimaPosicionConocidaJugador;
    public float velocidad = 2f;
    public float velocidadRotacion = 0.01f;
    public int waypointObjetivo = 0;
    public float distanciaWaypoint = 0.2f;
    public Transform objetivoActual;
    public Transform[] Waypoints;
    void Start()
    {
        objetivoActual = Waypoints[waypointObjetivo];

    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        ComprobarWaypoint();
    }

    void Movimiento(){
        Vector3 direccion = (objetivoActual.transform.position-this.transform.position).normalized;
        Quaternion rotacion = Quaternion.LookRotation(direccion,transform.up);
        this.transform.position = this.transform.position + direccion * velocidad * Time.deltaTime;
        transform.rotation = Quaternion.Lerp( this.transform.rotation, rotacion, Time.time *velocidadRotacion);
    }
    void ComprobarWaypoint(){
        float dist = Vector3.Distance(objetivoActual.position, this.transform.position);
        if(dist < distanciaWaypoint){
            if(waypointObjetivo+1 == Waypoints.Length){
                waypointObjetivo = 0;
            }
            else{
                waypointObjetivo+=1;
            }
            objetivoActual = Waypoints[waypointObjetivo];
        }
    }

    void AvisarCazador(GameObject cazador){
       // Movimiento(objetivoActual = cazador);
       cazador.GetComponent<CazadorMovement>().AvisoDeFantasma(ultimaPosicionConocidaJugador);
    }

    void OnTriggerEnter(Collision col){
         if (col.gameObject.name == "cazador"){
             AvisarCazador(col.gameObject);
         }
    }

    // funcion del cazador
    void AvisoDeFantasma(){

    }

    //funcion gargola
    void AvisarFantasma(){

    }
}
