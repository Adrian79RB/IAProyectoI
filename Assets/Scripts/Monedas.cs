using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monedas : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private ActivarSalida activarSalida;
    private GameObject cazadorPadre;
    private GameObject fantasmaPadre;

    void Start()
    {
        activarSalida = FindObjectOfType<ActivarSalida>();
        cazadorPadre = GameObject.Find("Cazadores");
        fantasmaPadre = GameObject.Find("Fantasmas");
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
        if (cazadorPadre.transform.GetChild(0).GetComponent<CazadorMovement>().consultaEstadoCazador() == EstadoNPC.GoingPatrol)
        {
            for (int i = 0; i < cazadorPadre.transform.childCount; i++)
            {
                cazadorPadre.transform.GetChild(i).GetComponent<CazadorMovement>().AvisoDeMonedas();
            }
        }

        for (int i = 0; i < fantasmaPadre.transform.childCount; i++)
        {
            fantasmaPadre.transform.GetChild(i).GetComponent<MovimientoFantasmas>().AvisoDeMonedas();
        }
    }
}
