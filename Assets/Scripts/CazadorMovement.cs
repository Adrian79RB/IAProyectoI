using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CazadorMovement : MonoBehaviour
{
    public Transform player;
    public GameObject waypoints;

    [SerializeField]Queue<Transform> pathStack;
    float speed = 3.0f;
    float rotationTime = 0.1f;
    Transform target = null;
    bool pathFound = false;

    // Start is called before the first frame update
    void Start()
    {
        target = transform;
        waypoints = GameObject.Find("Waypoints");
        pathStack = new Queue<Transform>();
    }

    private void Update()
    {
        if (!pathFound) {
            float pastDistanceToMe = 0.0f;
            float pastDistanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            for (int i = 0; i < waypoints.transform.childCount; i++) {
                Transform currentWaypoint = waypoints.transform.GetChild(i);

                if (Vector3.Distance(transform.position, currentWaypoint.position) > pastDistanceToMe
                    && Vector3.Distance(player.transform.position, currentWaypoint.position) < pastDistanceToPlayer) {
                    pathStack.Enqueue(currentWaypoint);
                    pastDistanceToMe = Vector3.Distance(transform.position, currentWaypoint.position);
                    pastDistanceToPlayer = Vector3.Distance(player.transform.position, currentWaypoint.position);
                }
            }

            foreach (Transform waypoint in pathStack)
                Debug.Log("Waypoints: " + waypoint.name);

            pathFound = true;
        }
    }


    void FixedUpdate()
    {
        if (pathFound) {
            if (Vector3.Distance(target.position, transform.position) < 0.5f) {

                if (pathStack.Count <= 0 || Vector3.Distance(target.position, transform.position) > Vector3.Distance(player.position, transform.position))
                    target = player;
                else
                {
                    target = pathStack.Peek();
                    pathStack.Dequeue();
                }

                Debug.Log("Target: " + target.name);
            }
            
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
