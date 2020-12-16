﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class voxelMesh : MonoBehaviour
{
    public Vector3Int size;
    public int[,,] cubes;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private List<int> tris=new List<int>();
    private List<Vector3> normals=new List<Vector3>();
    private List<Vector2> uvs=new List<Vector2>();
    public voxelMesh rightM,leftM,forwardM,backM;
    private int index=0;
    private float bigDetailMultiplay=0.005f;
    private float detailMultiplay=0.025f;
    private float detailHeightMultiplay=40;
    private int mapHeight=10;


    void Awake()
    {
        size=new Vector3Int(editVoxel.size,editVoxel.height,editVoxel.size);

        cubes=new int[size.x,size.y,size.z];

        meshFilter = GetComponent<MeshFilter>();

        meshCollider = GetComponent<MeshCollider>();

        for (int x=0;x<size.x;x++){
            for (int y=0;y<size.y;y++){
                for (int z=0;z<size.z;z++){

                    float bigDetail=Mathf.PerlinNoise((x+transform.position.x-99999)*bigDetailMultiplay,(z+transform.position.z-99999)*bigDetailMultiplay);

                    int maxHeight=Mathf.FloorToInt(Mathf.PerlinNoise((x+transform.position.x-99999)*detailMultiplay,(z+transform.position.z-99999)*detailMultiplay)*detailHeightMultiplay*bigDetail);

                    if(y<=maxHeight+mapHeight){
                        cubes[x,y,z]=1;
                    }
                    

                }
            }
        }
        createMesh();
    }
    
    void createFace(Vector3 normal){
        int[] trisA={
            index+0,index+1,index+2,
            index+2,index+1,index+3
        };
        Vector3[] normalsA={
            normal,
            normal,
            normal,
            normal
        };
        Vector2[] uvsA={
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(0,1)
        };
                            
        tris.AddRange(trisA);
        normals.AddRange(normalsA);
        uvs.AddRange(uvsA);
        index+=4;
    }
    public void createMesh(){
        Mesh mesh=new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        List<Vector3> vertices=new List<Vector3>();

        for (int x=0;x<size.x;x++){
            for (int y=0;y<size.y;y++){
                for (int z=0;z<size.z;z++){
                    if(cubes[x,y,z]!=0){
                        
                        //right
                        bool right=false;
                        if(x!=size.x-1){
                            if(cubes[x+1,y,z]==0){
                                right=true;
                            }
                        }else if(rightM!=null){
                                right=rightM.cubes[0,y,z]==0;

                        }else{
                            float bigDetail=Mathf.PerlinNoise((x+1+transform.position.x-99999)*bigDetailMultiplay,(z+transform.position.z-99999)*bigDetailMultiplay);
                            int maxHeight=Mathf.FloorToInt(Mathf.PerlinNoise((x+1+transform.position.x-99999)*detailMultiplay,(z+transform.position.z-99999)*detailMultiplay)*detailHeightMultiplay*bigDetail);
                            right=y>maxHeight+mapHeight;

                        }
                        

                        //left
                        bool left=false;
                        if(x!=0){
                            if(cubes[x-1,y,z]==0){
                                left=true;
                            }
                        }else if(leftM!=null){
                            left=leftM.cubes[size.x-1,y,z]==0;

                        }else{
                            float bigDetail=Mathf.PerlinNoise((x-1+transform.position.x-99999)*bigDetailMultiplay,(z+transform.position.z-99999)*bigDetailMultiplay);
                            int maxHeight=Mathf.FloorToInt(Mathf.PerlinNoise((x-1+transform.position.x-99999)*detailMultiplay,(z+transform.position.z-99999)*detailMultiplay)*detailHeightMultiplay*bigDetail);
                            left=y>maxHeight+mapHeight;
                        }

                        //up
                        bool up=false;
                        if(y!=size.y-1){
                            if(cubes[x,y+1,z]==0){
                                up=true;
                            }
                        }else{
                            up=true;
                        }
                        
                        //down
                        bool down=false;
                        if(y!=0){
                            if(cubes[x,y-1,z]==0){
                                down=true;
                            }
                        }

                        //forward
                        bool forward=false;
                        if(z!=size.z-1){
                            if(cubes[x,y,z+1]==0){
                                forward=true;
                            }
                        }else if(forwardM!=null){
                            forward=forwardM.cubes[x,y,0]==0;

                        }else{
                            float bigDetail=Mathf.PerlinNoise((x+transform.position.x-99999)*bigDetailMultiplay,(z+1+transform.position.z-99999)*bigDetailMultiplay);
                            int maxHeight=Mathf.FloorToInt(Mathf.PerlinNoise((x+transform.position.x-99999)*detailMultiplay,(z+1+transform.position.z-99999)*detailMultiplay)*detailHeightMultiplay*bigDetail);
                            forward=y>maxHeight+mapHeight;
                        }

                        //back
                        bool back=false;
                        if(z!=0){
                            if(cubes[x,y,z-1]==0){
                                back=true;
                            }
                        }else if(backM!=null){
                            back=backM.cubes[x,y,size.z-1]==0;

                        }else{
                            float bigDetail=Mathf.PerlinNoise((x+transform.position.x-99999)*bigDetailMultiplay,(z-1+transform.position.z-99999)*bigDetailMultiplay);
                            int maxHeight=Mathf.FloorToInt(Mathf.PerlinNoise((x+transform.position.x-99999)*detailMultiplay,(z-1+transform.position.z-99999)*detailMultiplay)*detailHeightMultiplay*bigDetail);
                            back=y>maxHeight+mapHeight;
                        }


                        if(right){
                            Vector3[] verticesA={
                                new Vector3(x+0.5f,y+0.5f,z-0.5f),
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x+0.5f,y-0.5f,z-0.5f),
                                new Vector3(x+0.5f,y-0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.right);
                        }
                        
                        if(left){
                            Vector3[] verticesA={
                                new Vector3(x-0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z-0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z-0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.left);
                        }
                        
                        if(up){
                            Vector3[] verticesA={
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x+0.5f,y+0.5f,z-0.5f),
                                new Vector3(x-0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z-0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.up);
                        }
                        
                        if(down){
                                Vector3[] verticesA={
                                new Vector3(x+0.5f,y-0.5f,z-0.5f),
                                new Vector3(x+0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z-0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.down);
                        }
                        
                        if(forward){
                            Vector3[] verticesA={
                                new Vector3(x+0.5f,y-0.5f,z+0.5f),
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.forward);
                        }
                        
                        
                        if(back){
                            Vector3[] verticesA={
                                new Vector3(x+0.5f,y+0.5f,z-0.5f),
                                new Vector3(x+0.5f,y-0.5f,z-0.5f),
                                new Vector3(x-0.5f,y+0.5f,z-0.5f),
                                new Vector3(x-0.5f,y-0.5f,z-0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.forward);
                        }
                    }
                }
            }
        }
        
        mesh.vertices=vertices.ToArray();
        mesh.normals=normals.ToArray();
        mesh.uv=uvs.ToArray();
        mesh.triangles=tris.ToArray();
        meshFilter.mesh=mesh;
        meshCollider.sharedMesh=mesh;

        index=0;
        tris.Clear();
        normals.Clear();
        uvs.Clear();
    }
}
