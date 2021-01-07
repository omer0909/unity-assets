using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class carController : MonoBehaviour
{
    [SerializeField]
    private Transform[] whweels;
    public float motorTorque=500;
    public float brakeTorque=1000;
    public float angle=30;
    [SerializeField]
    private WheelCollider[] whweelColliders;
    
    void FixedUpdate()
    {
        
        for(int i=0;i<whweelColliders.Length;i++){
            Vector3 pos;
            Quaternion rot;
            whweelColliders[i].GetWorldPose(out pos,out rot);
            whweels[i].rotation=rot;
            whweels[i].position=pos;

            whweelColliders[i].motorTorque=motorTorque*Input.GetAxis("Vertical");
            whweelColliders[i].brakeTorque=brakeTorque*Input.GetAxis("Jump");
            if(i<2){
                whweelColliders[i].steerAngle=angle*Input.GetAxis("Horizontal");
            }
        }
    }
}
