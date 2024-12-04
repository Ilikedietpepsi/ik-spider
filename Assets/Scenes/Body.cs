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

    public float MAX_ANGLE = 80f; //Set this to a very small angle to see results with too much constraints.

    public bool constraint = true; //Set this to false to see results with no constraints at all.

    

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
                legs[i].setPrevTarget(hit.point);
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

    public float getMaxAngle()
    {
        return this.MAX_ANGLE;
    }

    public bool getConstraint()
    {
        return this.constraint;
    }

    void FixedUpdate()
    {
        float height = 0f;//initial adjusted height
        Vector3 averageNormal = Vector3.zero;
        int groundedLegs = 0;

        for(int i=0; i<targets.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(targets[i].position, new Vector3(0f, -1f, 0f), out hit, detect_distance, surface))
            {
                height += hit.point.y;//height of each hit point
                averageNormal += hit.normal;
                groundedLegs++;
                if ((hit.point - legs[i].target).magnitude > 2f)
                {
                    if ((i%2==0)==even_step)
                    {
                        
                        legs[i].SetTarget(hit.point);
                    }
                    
                }
                

            }
        }

        if (groundedLegs > 0)
        {
            averageNormal /= groundedLegs;

            float targetHeight = 2.82f + height / groundedLegs;
            float smoothSpeed = 5f;
            float newY = Mathf.Lerp(transform.position.y, targetHeight, Time.deltaTime * smoothSpeed);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;
            float rotationSmoothSpeed = 5f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }

        even_step = !even_step;

 
    }

    void Update()
    {   
        transform.position += -transform.right * 3f * Time.deltaTime * Input.GetAxis("Vertical");
        transform.Rotate(0, 50f* Time.deltaTime * Input.GetAxis("Horizontal"), 0);
    }
}
