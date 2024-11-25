using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public int ITER_NUM = 5;
    public Transform spiderBody;
    public Transform[] joints;
    
    public Vector3 target;
    
    private Vector3 targetPosUp;
    private Vector3 prevTarget;
    private Vector3[] initialDirections = new Vector3[3];
    private const float MAX_ANGLE = 45f;

    public int leg_num;
    private void Awake() {
        spiderBody = this.transform.parent;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(this.prevTarget, targetPosUp) > 0.1f)
        {
            prevTarget = Vector3.Slerp(prevTarget, targetPosUp, 10f * Time.deltaTime);
        }
        else 
        {
            
            targetPosUp = target;
        }
        //force some position constraints
        if (leg_num % 2 ==0) {
            joints[1].transform.position += new Vector3(0,0,1) * spiderBody.localScale.z;
            joints[2].transform.position += new Vector3(0,0,1) * spiderBody.localScale.z;
        }
        if (leg_num % 2 ==1) {
            joints[1].transform.position += new Vector3(0,0,-1) * spiderBody.localScale.z;
            joints[2].transform.position += new Vector3(0,0,-1) * spiderBody.localScale.z;
        }
        joints[1].transform.position += new Vector3(0,1,0) * spiderBody.localScale.y;
        joints[2].transform.position += new Vector3(0,1,0) * spiderBody.localScale.y;
        joints[2].transform.position = new Vector3(joints[1].transform.position.x, joints[2].transform.position.y, joints[2].transform.position.z);
        for (int i=0; i<ITER_NUM; i++)
            FABRIK(prevTarget);
    }
    public void SetTarget(Vector3 pos)
    {
        target = pos;
        targetPosUp = target + new Vector3(0, 1, 0);
       
    }

    Vector3 Constraint(Vector3 prev, Vector3 desired, Vector3 new_joint, Vector3 prev_joint, float max_angle)
    {

        // Step 2: Normalize both vectors
        Vector3 prev_normalized = prev.normalized;
        Vector3 desired_normalized = desired.normalized;

        // Step 3: Calculate the dot product
        float dotProduct = Vector3.Dot(prev_normalized, desired_normalized);

        // Step 4: Compute the angle in radians
        float angleRadians = Mathf.Acos(Mathf.Clamp(dotProduct, -1.0f, 1.0f));

        // Step 5: Convert to degrees (optional)
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (angleDegrees > max_angle) {
            float link = 2f;
            // Project the point onto the cone
            
            Vector3 projectedPoint = ProjectPointOntoCone(prev_joint, new_joint, desired, prev_normalized, max_angle, link);


            return projectedPoint;
        }

        return new_joint;
    }

    private Vector3 ProjectPointOntoCone(Vector3 prev_joint, Vector3 new_joint, Vector3 point, Vector3 axis, float angleDegrees, float link)
    {
        // GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // cylinder.transform.localScale = new Vector3(0.2f, 2f, 0.2f);
        // Quaternion rotation = Quaternion.FromToRotation(Vector3.up, axis);
        // cylinder.transform.rotation = rotation; 
        // cylinder.transform.position = prev_joint + axis*1f;
        Vector3 A = prev_joint;
        Vector3 C = new_joint;
        Vector3 B = prev_joint + axis * link * Mathf.Cos(angleDegrees*Mathf.PI/180);
        Vector3 BA = B - A;
        Vector3 CA = C-A;
        float magnitudeAB = BA.sqrMagnitude;
        float dotProduct = Vector3.Dot(CA, BA);
        float projectionScalar = dotProduct / magnitudeAB;

        Vector3 projection = BA * projectionScalar;
        Vector3 P = A + projection;
        return B + (C - P).normalized * Mathf.Tan((angleDegrees * Mathf.PI / 180)) * BA.magnitude;
    }
    void FABRIK(Vector3 target) {

        Vector3 temp = target;
        Vector3 start = joints[0].position;
        

        for (int i = 2; i >=0; i--) {
            
            Vector3 desired = joints[i].position - temp;
            
            Vector3 new_joint = temp + desired.normalized * 2f;
            // // just to show what would happen if we add too much constraints
            // if (i==2) {
            //     joints[i].position = new_joint;
            // }
            // if (i!=2) {
            //     joints[i].position = Constraint(initialDirections[i+1], desired, new_joint, joints[i+1].position, 80f);
            // }
            joints[i].position = new_joint;
    
            

            initialDirections[i] = joints[i].position - temp;

            joints[i].LookAt(temp);
            temp = joints[i].position;
        }

        joints[0].position = start;
        for(int i=0; i<3; i++)
        {
            if (i == 2) {
                joints[i].LookAt(target);
                //joints[i].LookAt(joints[i].position - new Vector3(0f, 2f, 0f));
            }
            

                
            else
            {
                 
                Vector3 desired = joints[i + 1].position - joints[i].position;
                float link_length = 2f; 
                Vector3 new_joint = joints[i].position + desired.normalized * link_length;
                float max_angle = 0f;
                if (i==0) {
                    max_angle = 10f;
                }
                if (i==1) {
                    max_angle = 10f;
                }
                Vector3 prev = joints[i + 1].position;
                
                
                joints[i + 1].position = Constraint(-initialDirections[i], desired, new_joint, joints[i].position, max_angle);
                //joints[i + 1].position = new_joint;
                joints[i].LookAt(joints[i+1].position);
            }
            
            
        }
    }


    
}
