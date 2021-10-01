using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public List<float> weigths;
    public List<GameObject> arcs;
    
    int numRayos = 7;

    // Start is called before the first frame update
    void Start()
    {
        weigths = new List<float>();
        arcs = new List<GameObject>();

        //Lanzamiento de rayos para construir el grafo
        RaycastHit ray;
        for (int i = 0; i < numRayos; i++)
        {
            //Vector3 origin = new Vector3( transform.position.x - i/2 * bCollider.size.x / 2, transform.position.y, transform.position.z - (i+1)/2 * bCollider.size.z / 2);
            float angle = (Mathf.PI / 4 * i) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, 0, 1)), out ray, 100.0f) && ray.transform.tag == "Waypoint")
            {
                Debug.Log("Tag: " + ray.transform.tag + " Position: " + ray.transform.position);
                arcs.Add(ray.transform.gameObject);
                weigths.Add(Vector3.Distance(transform.position, ray.transform.position));
            }
        }

    }
}
