using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class sculpt : MonoBehaviour
{
    private Vector2 oldMousePos;
    public float size=0.4f;
    public float force=0.1f;
    public float spacing=100;
    private float slope=1;
    public AnimationCurve curve;
    Mesh mesh;
    MeshCollider mCollider;
    void Awake() {
        if (curve.length==0){
            curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            curve.preWrapMode = WrapMode.PingPong;
            curve.postWrapMode = WrapMode.PingPong;
        }
        mesh=GetComponent<MeshFilter>().mesh;
        mCollider=GetComponent<MeshCollider>();
        mCollider.sharedMesh=mesh;
    }
    
    void point(Vector3 pos,Vector3 normal){
        Vector3[] vertices=mesh.vertices;
                    
        for(int v=0;v<vertices.Length;v++){
            float distance =Vector3.Distance(vertices[v],pos);
            if(distance < size){
                vertices[v]=vertices[v]+normal*force*size*curve.Evaluate((1-(distance/size)));
            }
        }
        mesh.vertices=vertices;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            force*=-1;
        }
        if(Input.GetKeyUp(KeyCode.LeftControl)){
            force*=-1;
        }

        if(Input.GetMouseButtonDown(0)){
            oldMousePos=Input.mousePosition;

            Ray ray=Camera.main.ScreenPointToRay(oldMousePos);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){       
                point(hit.point,hit.normal);
                mesh.RecalculateNormals();
                mCollider.sharedMesh=mesh;
            }
        }

        if (Input.GetMouseButton(0)){
            Vector2 mousepos=Input.mousePosition;
            Vector2 mousedelta=mousepos-oldMousePos;

            Transform camera=Camera.main.transform;
            Vector3 mouseMove=camera.right*mousedelta.x+camera.up*mousedelta.y;
            mouseMove.Normalize();
            
            while(true){
                if(Vector2.Distance(mousepos,oldMousePos)>size*spacing){
                    oldMousePos=Vector2.MoveTowards(oldMousePos,mousepos,size*spacing*slope);
                    
                    Ray ray=Camera.main.ScreenPointToRay(oldMousePos);
                    RaycastHit hit;
                    if(Physics.Raycast(ray,out hit)){
                        slope=1-Mathf.Abs(Vector3.Dot(hit.normal,mouseMove));
                        point(hit.point,hit.normal);
                    }

                }else{
                    mesh.RecalculateNormals();
                    break;
                }
            }  
        }
        if(Input.GetMouseButtonUp(0)){
            mCollider.sharedMesh=mesh;
        }
    }
}
