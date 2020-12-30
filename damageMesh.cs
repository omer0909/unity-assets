using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageMesh : MonoBehaviour
{
    private List<GameObject> objects=new List<GameObject>();
    public float radius=0.2f;
    MeshFilter mesh;
    //MeshCollider meshCollider;
    void Awake(){
        mesh=GetComponent<MeshFilter>();
        //meshCollider=GetComponent<MeshCollider>();
    }
    void OnCollisionEnter(Collision collisionInfo)
    {
        if(!objects.Contains(collisionInfo.gameObject)){
            objects.Add(collisionInfo.gameObject);
            Vector3[] vertices=mesh.mesh.vertices;
            
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    float distance=Vector3.Distance(transform.worldToLocalMatrix.MultiplyPoint3x4(contact.point),vertices[i]);
                    if(distance<radius){
                        vertices[i]=vertices[i]+(transform.InverseTransformDirection(contact.normal)*(radius-distance)*collisionInfo.relativeVelocity.magnitude*0.01f);
                    }
                    
                }
            }
            mesh.mesh.vertices=vertices;
            mesh.mesh.RecalculateNormals();
            //meshCollider.sharedMesh=mesh.mesh;
        }
    }
}
