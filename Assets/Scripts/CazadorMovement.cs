using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CazadorMovement : MonoBehaviour
{
    public Transform player;

    Rigidbody rgbd;
    float speed = 3.0f;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = player;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = rotation;
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
