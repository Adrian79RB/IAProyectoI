using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mano : MonoBehaviour
{
    public Transform jugador;

    public Animator animator;
    public float temporizador = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        temporizador -= Time.deltaTime;
        if(Input.anyKey){
            temporizador = 5;
        }
        this.transform.position = jugador.position + new Vector3(0,4,0);
        if(temporizador <0){
            animator.SetTrigger("Aplastar");
        }
    }
}
