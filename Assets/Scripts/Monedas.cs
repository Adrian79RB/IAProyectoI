using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monedas : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private ActivarSalida activarSalida;

    void Start()
    {
        activarSalida = FindObjectOfType<ActivarSalida>();
       
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("Contador = " + activarSalida.contadorMonedas);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            this.gameObject.SetActive(false);

            activarSalida.contadorMonedas++;
          
            
        }
    }
}
