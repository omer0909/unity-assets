using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class meshPoint : MonoBehaviour
{
    private MeshFilter meshFilter;
    public float size=0.1f;
    public int maxIndex=15000;
    public float margin=0.01f;
    private int index=0;
    void Awake()
    {
        if(15000<maxIndex){
            Debug.LogError("maxIndex cannot be greater than 1500");
        }
        
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh=new Mesh();

    }
    void createmesh(Vector3 pos,Vector3 normal){
        Mesh mesh=new Mesh();
        Vector3[] vertices={
            new Vector3(-size,-size,margin),
            new Vector3(size,-size,margin),
            new Vector3(-size,size,margin),
            new Vector3(size,size,margin),
        };
        Vector3[] normals=new Vector3[vertices.Length];

        Quaternion rotation=Quaternion.LookRotation(normal,Vector3.up);
        for(int i=0;i<vertices.Length;i++){
            
            vertices[i]=rotation*vertices[i];
            normals[i]=normal;
            vertices[i]+=pos;
        }

        mesh.vertices=vertices;
        mesh.normals=normals;

        int[] tris = {
            0,1,2,
            2,1,3
        };

        mesh.triangles=tris;

        Vector2[] uv = {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(1,1),
        };

        mesh.uv=uv;
        
        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = meshFilter.sharedMesh;
        combine[1].mesh = mesh;
        combine[0].transform = transform.localToWorldMatrix;
        combine[1].transform = transform.localToWorldMatrix;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);
        
    }
    void editMesh(Vector3 pos,Vector3 normal){

        Vector3[] vertices=meshFilter.mesh.vertices;
        Vector3[] normals=meshFilter.mesh.normals;

        vertices[index*4]=new Vector3(-size,-size,margin);
        vertices[index*4+1]=new Vector3(size,-size,margin);
        vertices[index*4+2]=new Vector3(-size,size,margin);
        vertices[index*4+3]=new Vector3(size,size,margin);

        Quaternion rotation=Quaternion.LookRotation(normal,Vector3.up);
        for(int i=0;i<4;i++){

            vertices[index*4+i]=rotation*vertices[index*4+i];
            normals[index*4+i]=normal;
            vertices[index*4+i]+=pos;
        }

        meshFilter.mesh.vertices=vertices;
        meshFilter.mesh.normals=normals;
    }

    void Update()
    {
        if(Input.GetMouseButton(0)){
            
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast( ray,out hit)){
                

                if(meshFilter.mesh.vertices.Length<maxIndex*4){

                    createmesh(hit.point,hit.normal);

                }else{

                    editMesh(hit.point,hit.normal);
                    
                    index++;
                    index=(index+1>maxIndex)?0:index;
                }
            }
                    
        }
        
    }
}
