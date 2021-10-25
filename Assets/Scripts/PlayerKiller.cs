using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
    public GameObject cazado;

    Transform player;
    bool IsPlayerInRange;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("JohnLemon").transform;
        IsPlayerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            IsPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform == player)
        {
            IsPlayerInRange = false;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInRange)
        {
            Vector3 direccion = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direccion);
            RaycastHit raycastHit;
            Debug.DrawRay(transform.position, direccion);
            if(Physics.Raycast(ray, out raycastHit))
            {
                if(raycastHit.collider.transform == player)
                {
                    cazado.GetComponent<GameEnding>().CaughtPlayer();
                }
            }
        }
    }
}
