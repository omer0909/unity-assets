using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class createSnow : MonoBehaviour
{
    private MeshFilter meshFilter;
    public int matrixSize=200;
    private float size=1;
    private float convert;
    public float depth=0.2f;
    private Vector3[] vertices;
    private Vector3[] normals;
    void Awake()
    {
        
        float squareSize=size/(matrixSize-1);
        float uvSize=1f/(matrixSize-1);

        meshFilter=GetComponent<MeshFilter>();
        Mesh newMesh=new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int vertexLegenth=matrixSize*matrixSize;

        vertices=new Vector3[vertexLegenth];
        for(int i=0;i<vertices.Length;i++){
            vertices[i]=new Vector3((i%matrixSize)*squareSize,depth,(i/matrixSize)*squareSize);
        }

        Vector2[] uv=new Vector2[vertexLegenth];
        for(int i=0;i<uv.Length;i++){
            uv[i]=new Vector2((i%matrixSize)*uvSize,(i/matrixSize)*uvSize);
        }

        normals=new Vector3[vertexLegenth];
        for(int i=0;i<normals.Length;i++){
            normals[i]=Vector3.up;
        }

        int[] triangles=new int[(matrixSize-1)*(matrixSize-1)*6];
        for(int i=0;i<triangles.Length/6;i++){
            int d=i+(i/(matrixSize-1));
            int[] corners={
                d,d+1,d+matrixSize,d+1+matrixSize
            };
            int trisIndex=i*6;
            triangles[trisIndex]=corners[0];
            triangles[trisIndex+1]=corners[2];
            triangles[trisIndex+2]=corners[1];

            triangles[trisIndex+3]=corners[1];
            triangles[trisIndex+4]=corners[2];
            triangles[trisIndex+5]=corners[3];
        }

        newMesh.vertices=vertices;
        newMesh.normals=normals;
        newMesh.uv=uv;
        newMesh.triangles=triangles;
        meshFilter.mesh=newMesh;

        size=transform.localScale.x;
        convert=(1/size)*(matrixSize-1);
    }
    int calculateIndex(Vector2Int pos){
        int index=pos.x*matrixSize+pos.y;
        return index;
    }
    Vector3 trisNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        return Vector3.Cross(side1, side2).normalized;
    }
    void calculateNormal(int index){
        Vector3[] c={
                vertices[index],vertices[index+1],vertices[index-1],vertices[index+matrixSize],vertices[index-matrixSize]
            };
        Vector3 normal=(trisNormal(c[0], c[3], c[1])+trisNormal(c[0], c[2], c[3])+trisNormal(c[0], c[4], c[2])+trisNormal(c[0], c[1], c[4])).normalized;

        normals[index]= normal;
    }
    public void edit(Vector2 pos,float r)
    {
        int legenth=(int)(r*2*size);
        Vector2Int startIndex=new Vector2Int((int)(pos.x*convert)-(int)legenth/2,(int)(pos.y*convert)-(int)legenth/2);
        
        for (int x=0;x<legenth;x++){
            for(int y=0;y<legenth;y++){

                Vector2Int indexVector=startIndex+new Vector2Int(x,y);
                float distance=Vector2.Distance(pos*convert,indexVector);

                if(distance<r*convert){
                    int index=calculateIndex(indexVector);
                    vertices[index].y=Mathf.Max(vertices[index].y-(0.5f*depth*(r*convert-distance)),-depth);
                }
            }
        }
        for (int x=0;x<legenth+2;x++){
            for(int y=0;y<legenth+2;y++){

                Vector2Int indexVector=startIndex+new Vector2Int(x,y)-Vector2Int.one;
                float distance=Vector2.Distance(pos*convert,indexVector);
                if(distance<r*convert){
                    calculateNormal(calculateIndex(indexVector));

                    int index=calculateIndex(indexVector);
                    if(normals[index]==Vector3.zero){
                        normals[index]=Vector3.up;
                    }
                }
            }
        }
        meshFilter.mesh.vertices=vertices;
        meshFilter.mesh.normals=normals;
    }
    /*
    void Update(){
        if(Input.GetMouseButton(0)){
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit hit)){
                edit(new Vector2(hit.point.z,hit.point.x),0.5f);
            }
        }
    }
    */
}
