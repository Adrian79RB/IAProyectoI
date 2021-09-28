using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivarSalida : MonoBehaviour
{
    // Start is called before the first frame update
    public int contadorMonedas;
    public GameObject Salida;
  
    void Start()
    {
        contadorMonedas = 0;
        Salida.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(contadorMonedas >= 2)
        {
            Salida.gameObject.SetActive(true);
            Debug.Log("Holaaaaaaaaaaaaa");
        }
    }



}
