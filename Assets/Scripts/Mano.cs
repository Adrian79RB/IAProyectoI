using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mano : MonoBehaviour
{
    public GameEnding cazado;
    public Transform jugador;
    public Animator animator;
    public float temporizador = 5;

    bool handActive = false;

    // Update is called once per frame
    void Update()
    {
        temporizador -= Time.deltaTime;
        if(Input.anyKey){
            temporizador = 5;
        }
        this.transform.position = jugador.position + new Vector3(0,4,0);
        if(temporizador < 0){
            animator.SetTrigger("Aplastar");
            StartCoroutine("ActivateHand");
        }
    }

    IEnumerator ActivateHand()
    {
        yield return new WaitForSeconds(1.5f);
        handActive = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (handActive && other.tag == "Player")
        {
            cazado.CaughtPlayer();
        }
    }
}
