using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private List<Transform> targets = new List<Transform>();


    private Vector3 prevPos;

    public bool step = false;

    public bool even_step = true;
    private Leg[] legs;
    // Start is called before the first frame update
    void Start()
    {
        legs = GetComponentsInChildren<Leg>();

        foreach (Leg leg in legs)
        {
            GameObject leg_target = new GameObject(leg.name + "_target");
            leg_target.transform.SetParent(this.transform);

            Vector3 direction = leg.transform.GetChild(2).position - this.transform.position;
            Vector3 pos = this.transform.position + direction;
            leg_target.transform.position = new Vector3(pos.x, 0, pos.z);
            targets.Add(leg_target.transform);
        }
        
        for (int i = 0; i < legs.Length; i++)
        {
            
            legs[i].SetTargetPos(targets[i].transform.position);

        }
    }

    void FixedUpdate()
    {
        
        for (int i = 0; i < legs.Length; i++)
        {
            
            Vector3 new_target = new Vector3(targets[i].position.x, 0, targets[i].position.z);
            float distance = Vector3.Distance(new_target, legs[i].targetPos);
            if (distance > 1f)
            {
                step = true;
                if ((i%2==0)==even_step) {
                    Debug.Log(i);
                    legs[i].SetTargetPos(new_target + (transform.position - prevPos)*10f);
                
                }
            
            }
        }
        even_step = !even_step;
        prevPos = transform.position;
 
    }
}
