using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public int FABRIK_ITT = 3; // Accuracy of inverse kinematics
    public Transform spiderBody;
    public Transform[] joints;
    // public Transform target;
    

    public Vector3 targetPos;
    
    private Vector3 targetPosUp;
    private Vector3 prevTargetPos;
    
    private void Awake() {
        spiderBody = this.transform.parent;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(this.prevTargetPos, targetPosUp) > 0.1f)
        {
            prevTargetPos = Vector3.Slerp(prevTargetPos, targetPosUp, 10f * Time.deltaTime);
        }
        else 
        {
            
            targetPosUp = targetPos;
        }
        for (int i=0; i<FABRIK_ITT; i++)
            FABRIK(prevTargetPos);
    }
    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
        targetPosUp = pos + new Vector3(0, 1, 0);
       
    }
    void FABRIK(Vector3 target, bool backwardsOnly=false) {
        Vector3 temp = target;
        Vector3 start = joints[0].position;

        for (int i = 2; i >=0; i--) {
            Vector3 direction = (joints[i].position - temp).normalized;
            
            float link_length = 2;
            joints[i].position = temp + direction * link_length;

            joints[i].LookAt(temp);

            temp = joints[i].position;
         
        }

        if (backwardsOnly)
            return;
        joints[0].position = start;
        for(int i=0; i<3; i++)
        {
            if (i == 2) {
                joints[i].LookAt(targetPos); // Look at targetPos
           
            }
                
            else
            {
                // Look at next joint
                joints[i].LookAt(joints[i+1].position);

                // Find the vector between joints
                Vector3 direction = (joints[i+1].position - joints[i].position).normalized;

                // Place the joint i+1 on the vector with respective linkLength
                // float linkLength = joints[i].GetChild(0).localScale.z;
                float link_length = 2;
                joints[i+1].position = joints[i].position + direction * link_length;
            
            }
        }
    }

    


    
}
