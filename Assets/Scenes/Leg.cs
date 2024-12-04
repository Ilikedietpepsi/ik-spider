using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public Transform[] joints;
    
    public Vector3 target;
    
    private Vector3 prevTarget;
    private Vector3[] initialDirections = new Vector3[3];

    public int ITER_NUM = 5;
    
    public int leg_num; //Do not change this in the GUI if unsure.The right legs should have even number and left legs should have odd numbers.

    
    private Body spider;


    private void Awake() {
        if (spider == null)
        {
            spider = GetComponentInParent<Body>(); // Search for Body in parent hierarchy
        }
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(this.prevTarget, target) > 0.1f)
        {
            prevTarget = Vector3.Slerp(prevTarget, target, 10f * Time.deltaTime);
        }
        if (spider.getConstraint())
        {
            position_constraint();
        }
        
        for (int i=0; i<ITER_NUM; i++)
            FABRIK(prevTarget);
    }

    void position_constraint()
    {

        //force some position constraints
        joints[1].transform.position += (joints[1].transform.position - new Vector3(spider.transform.position.x, joints[1].transform.position.y, spider.transform.position.z)).normalized;
        joints[2].transform.position += (joints[1].transform.position - new Vector3(spider.transform.position.x, joints[2].transform.position.y, spider.transform.position.z)).normalized;
        
        joints[1].transform.position += new Vector3(0,1,0);
        joints[2].transform.position += new Vector3(0,1,0);
        joints[2].transform.position = new Vector3(joints[1].transform.position.x, joints[2].transform.position.y, joints[2].transform.position.z);
    }
    public void setPrevTarget(Vector3 pos) {
        prevTarget = pos;
    }
    public Transform GetJoint(int index)
    {
        return joints[index];
    }
    public void SetTarget(Vector3 pos)
    {
        target = pos;
    }

    Vector3 Constraint(Vector3 prev, Vector3 desired, Vector3 new_joint, Vector3 prev_joint, float max_angle)
    {

        
        Vector3 prev_normalized = prev.normalized;
        Vector3 desired_normalized = desired.normalized;


        float dotProduct = Vector3.Dot(prev_normalized, desired_normalized);

        float angleRadians = Mathf.Acos(Mathf.Clamp(dotProduct, -1.0f, 1.0f));

      
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (angleDegrees > max_angle) {
            float link = 2f;
            Vector3 projectedPoint = ProjectPointOntoCone(prev_joint, new_joint, desired, prev_normalized, max_angle, link);


            return projectedPoint;
        }

        return new_joint;
    }

    private Vector3 ProjectPointOntoCone(Vector3 prev_joint, Vector3 new_joint, Vector3 point, Vector3 axis, float angleDegrees, float link)
    {
        Debug.Log("constraining..");
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
            
            if (i==2) {
                joints[i].position = new_joint;
            }
            if (i!=2) {
                if (!spider.getConstraint())
                {
                    joints[i].position = new_joint;
                }
                else{
                    joints[i].position = Constraint(initialDirections[i+1], desired, new_joint, joints[i+1].position, spider.getMaxAngle());//SOME CONSTRAINTS
                }
                

            }
    
            

            initialDirections[i] = joints[i].position - temp;

            joints[i].LookAt(temp);
            temp = joints[i].position;
        }

        joints[0].position = start;
        for(int i=0; i<3; i++)
        {
            if (i == 2) {
                joints[i].LookAt(target);
            }
            

                
            else
            {
                 
                Vector3 desired = joints[i + 1].position - joints[i].position;
                Vector3 new_joint = joints[i].position + desired.normalized * 2f;
                Vector3 prev = joints[i + 1].position;
                
            
                joints[i + 1].position = new_joint;
                joints[i].LookAt(joints[i+1].position);
            }
            
            
        }
    }


    
}
