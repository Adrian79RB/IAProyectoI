using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivarSalida : MonoBehaviour
{
    // Start is called before the first frame update
    public int contadorMonedas;
    public GameObject Salida;
    public GameObject marcador;
    public bool textChange = false;
  
    void Start()
    {
        contadorMonedas = 0;
        Salida.gameObject.SetActive(false);
        marcador.transform.GetChild(1).GetComponent<Text>().text = "Coins:   / " + transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (textChange)
        {
            textChange = false;
            marcador.transform.GetChild(0).GetComponent<Text>().text = contadorMonedas.ToString();
        }

        if(contadorMonedas >= transform.childCount && !Salida.gameObject.activeSelf)
        {
            Salida.gameObject.SetActive(true);
        }
    }



}
