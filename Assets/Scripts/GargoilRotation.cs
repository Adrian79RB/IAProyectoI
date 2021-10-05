using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoilRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
         if (col.gameObject.tag == "fantasma"){
             AvisarFantasma(col.gameObject);
         }
    }

    void AvisarFantasma(GameObject fantasma){
       fantasma.GetComponent<MovimientoFantasmas>().AvisoDeGargola();
    }
}
