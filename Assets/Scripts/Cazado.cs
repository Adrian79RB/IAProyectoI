using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cazado : MonoBehaviour
{
    public GameObject player;
    [SerializeField]
    private GameEnding end;

    // Start is called before the first frame update
    void Start()
    {
        end = FindObjectOfType<GameEnding>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            end.CaughtPlayer();
        }
    }
}
