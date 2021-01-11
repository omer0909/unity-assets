using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(WheelCollider))]
public class wheel : MonoBehaviour
{
    public float trailRadius=0.2f;
    private Vector3 oldpos=Vector3.zero;
    private WheelCollider wheelCollider;
    void Awake()
    {
        wheelCollider=GetComponent<WheelCollider>();
    }
    void Update()
    {
        if(wheelCollider.GetGroundHit(out WheelHit hit)){
            if(hit.collider.tag=="snow"){
                //hit.collider.gameObject.GetComponent<ground>().edit(hit.point,-hit.normal,trailRadius);
                if(Vector3.Distance(oldpos,hit.point)>trailRadius*0.5f){
                    hit.collider.gameObject.GetComponent<createSnow>().edit(new Vector2(hit.point.z,hit.point.x),trailRadius);
                    oldpos=hit.point;
                }
            }
        }
    }
}
