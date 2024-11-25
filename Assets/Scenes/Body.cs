using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private float detect_distance = 2f;
    private List<Transform> targets = new List<Transform>();

    private bool even_step = true;
    private Leg[] legs;

    private LayerMask surface;
    void Awake()
    {
        surface = LayerMask.GetMask("Walkable");
        legs = GetComponentsInChildren<Leg>();

        foreach (Leg leg in legs)
        {
            Transform leg_target = createTarget(leg);
            targets.Add(leg_target.transform);
        }
        for(int i=0; i<targets.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(targets[i].position, new Vector3(0f, -1f, 0f), out hit, detect_distance, surface))
            {
                legs[i].SetTarget(hit.point);
                // GameObject leg_target = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // leg_target.name = $"{i}_target";

                // // Set the scale to (1,1,1)
                // leg_target.transform.localScale = new Vector3(1, 1, 1);
                // leg_target.transform.position = hit.point;

            }
        }
    }
    

    private Transform createTarget(Leg leg) {
        Vector3 direction = leg.transform.position - transform.position;
        
        GameObject leg_target = new GameObject($"{leg.name}_target");
        leg_target.transform.SetParent(this.transform);

        leg_target.transform.position = leg.transform.position + direction*2f;

        return leg_target.transform;
    }

    void FixedUpdate()
    {
        for(int i=0; i<targets.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(targets[i].position, new Vector3(0f, -1f, 0f), out hit, detect_distance, surface))
            {
                if ((hit.point - legs[i].target).magnitude > 2f)
                {
                    if ((i%2==0)==even_step)
                    {
                        legs[i].SetTarget(hit.point);
                        // GameObject target = GameObject.Find($"{i}_target");
                        // target.transform.position = hit.point;

                    }
                }
            }
        }
        even_step = !even_step;

 
    }

    void Update()
    {   
        transform.position += -transform.right * 3f * Time.deltaTime * Input.GetAxis("Vertical");
        transform.Rotate(0, 10f* Time.deltaTime * Input.GetAxis("Horizontal"), 0);
    }
}
