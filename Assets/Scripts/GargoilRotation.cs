using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoilRotation : MonoBehaviour
{
    public int flexibilidad = 0;
    public float timeCounter = 0f;
    public int etapa =0;
    public bool ida = true;
    public Transform gargola;

    // Update is called once per frame
    void Update()
    {
        if (timeCounter >= 10f){
            rotateGargoil();
            timeCounter = 0f;
        }
        timeCounter+=Time.deltaTime;
    }
    void rotateGargoil(){
        switch (flexibilidad)
            {
                case(4):
                    gargola.Rotate(0f,90f,0f);
                    break;
                case(3):
                    if(etapa == 3){
                        ida = false;
                        gargola.Rotate(0f,-90f,0f);
                        etapa = 2;
                        break;
                    }
                    if(etapa == 2){
                        if(ida){
                            etapa = 3;
                            gargola.Rotate(0f,90f,0f);
                            break;
                        }
                        else{
                            etapa = 1;
                            gargola.Rotate(0f,-90f,0f);
                            break;
                        }
                    }
                    if(etapa == 1){
                        if(ida){
                            etapa = 2;
                            gargola.Rotate(0f,90f,0f);
                            break;
                        }
                        else{
                            etapa = 0;
                            gargola.Rotate(0f,-90f,0f);
                            break;
                        }
                    }
                    if(etapa == 0){
                        ida = true;
                        etapa = 1;
                        gargola.Rotate(0f,90f,0f);
                        break;
                    }
                    break;
                case(2):
                    if(etapa == 2){
                        ida = false;
                        gargola.Rotate(0f,-90f,0f);
                        etapa = 1;
                        break;
                    }
                    if(etapa == 1){
                        if(ida){
                            etapa = 2;
                            gargola.Rotate(0f,90f,0f);
                            break;
                        }
                        else{
                            etapa = 0;
                            gargola.Rotate(0f,-90f,0f);
                            break;
                        }
                    }
                    if(etapa == 0){
                        ida = true;
                        etapa = 1;
                        gargola.Rotate(0f,90f,0f);
                        break;
                    }
                    break;
                case(1):
                    if(etapa == 1){
                        gargola.Rotate(0f,-90f,0f);
                        etapa = 0;
                        break;
                    }
                    else{
                        etapa = 1;
                        gargola.Rotate(0f,90f,0f);
                        break;
                    }

                default:
                    break;
            }
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
