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
            float angle = (Mathf.PI / 4 * i) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, 0, 1)), out ray, 100.0f) && ray.transform.tag == "Waypoint")
            {
                arcs.Add(ray.transform.gameObject);
                weigths.Add(Vector3.Distance(transform.position, ray.transform.position));
            }
        }

    }
}
