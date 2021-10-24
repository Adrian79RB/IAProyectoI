using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monedas : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private ActivarSalida activarSalida;
    private GameObject cazadorPadre;

    void Start()
    {
        activarSalida = FindObjectOfType<ActivarSalida>();
        cazadorPadre = GameObject.Find("Cazadores");
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Contador = " + activarSalida.contadorMonedas);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            this.gameObject.SetActive(false);

            activarSalida.contadorMonedas++;

            LanzarAvisoMonedas();
        }
    }

    void LanzarAvisoMonedas(){
        if (cazadorPadre.transform.GetChild(0).GetComponent<CazadorMovement>().consultaEstadoCazador() == EstadoNPC.Alerted)
        {
            for(int i = 0; i < cazadorPadre.childCount; i++){
                cazadorPadre.transform.GetChild(0).GetComponent<CazadorMovement>().AvisoDeMonedas();
            }
        }
    }
}
