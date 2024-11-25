using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private float detect_distance = 4f;
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
            }
        }
    }
    

    private Transform createTarget(Leg leg) {
        Vector3 direction = leg.transform.position - transform.position;
        
        GameObject leg_target = new GameObject($"{leg.name}_target");
        leg_target.transform.SetParent(this.transform);

        //leg_target.transform.position = leg.transform.position + direction*2f; //more natural, but not working for height adjustment

        leg_target.transform.position = leg.GetJoint(2).position; //less natural, but working for height adjustment

        return leg_target.transform;
    }

    void FixedUpdate()
    {
        float height = 0f;//initial adjusted height
        
        for(int i=0; i<targets.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(targets[i].position, new Vector3(0f, -1f, 0f), out hit, detect_distance, surface))
            {
                height += hit.point.y;//height of each hit point
                if ((hit.point - legs[i].target).magnitude > 2f)
                {
                    if ((i%2==0)==even_step)
                    {
                        
                        legs[i].SetTarget(hit.point);
                    }
                    
                }
                

            }
        }
        transform.position = new Vector3(transform.position.x, 2.82f+height / targets.Count,transform.position.z);//adjust the body height 
        even_step = !even_step;

 
    }

    void Update()
    {   
        transform.position += -transform.right * 3f * Time.deltaTime * Input.GetAxis("Vertical");
        transform.Rotate(0, 50f* Time.deltaTime * Input.GetAxis("Horizontal"), 0);
    }
}
