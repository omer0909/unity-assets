using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class vertexPaint : MonoBehaviour
{
    public Color32 color;
    private Vector2 oldMousePos;
    public float size=0.4f;
    public float spacing=100;
    private float slope=1;
    public AnimationCurve curve;
    private Mesh mesh;
    private MeshCollider mCollider;
    private Camera cam;
    private float distance;
    float distanceMultiplay;
    void Awake() {
        cam=Camera.main;
        distanceMultiplay=Mathf.Sin(cam.fieldOfView*0.5f*Mathf.Deg2Rad);

        if (curve.length==0){
            curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            curve.preWrapMode = WrapMode.PingPong;
            curve.postWrapMode = WrapMode.PingPong;
        }

        mesh=GetComponent<MeshFilter>().mesh;
        mCollider=GetComponent<MeshCollider>();
        mCollider.sharedMesh=mesh;
        
        if(mesh.colors.Length==0){
            mesh.colors=new Color[mesh.vertices.Length];
        }
    }

    void point(Vector3 pos){
        Vector3[] vertices=mesh.vertices;
        Color32[] colors=mesh.colors32;

        for(int v=0;v<vertices.Length;v++){
            float distanceV =Vector3.Distance(vertices[v],pos);
            if(distanceV < size){
                colors[v]=Color32.Lerp(colors[v],color,curve.Evaluate((1-(distanceV/size))));
            }
        }
        mesh.colors32=colors;
    }
    
    void Update()
    {

        if(Input.GetMouseButtonDown(0)){
            oldMousePos=Input.mousePosition;

            Ray ray=Camera.main.ScreenPointToRay(oldMousePos);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){ 
                point(hit.point);
                distance=hit.distance*distanceMultiplay;
            }
        }

        if (Input.GetMouseButton(0)){
            Vector2 mousepos=Input.mousePosition;
            Vector2 mousedelta=mousepos-oldMousePos;

            Transform camera=Camera.main.transform;
            Vector3 mouseMove=camera.right*mousedelta.x+camera.up*mousedelta.y;
            mouseMove.Normalize();

            while(true){
                if(Vector2.Distance(mousepos,oldMousePos)>size*spacing/distance){
                    oldMousePos=Vector2.MoveTowards(oldMousePos,mousepos,(size*spacing*slope)/distance);
                    
                    Ray ray=Camera.main.ScreenPointToRay(oldMousePos);
                    RaycastHit hit;
                    if(Physics.Raycast(ray,out hit)){
                        
                        slope=1.3f-Mathf.Abs(Vector3.Dot(hit.normal,mouseMove));
                        point(hit.point);
                        distance=hit.distance*distanceMultiplay;
                    }

                }else{
                    break;
                }
            }  
        }
    }
}
