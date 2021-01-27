using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class voxelMesh : MonoBehaviour
{
    private Vector3Int Size = new Vector3Int(50, 50, 50);
    public int[,,] cubes;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private List<int> tris = new List<int>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private int index = 0;
    bool add = true;

    void Awake()
    {
        cubes = new int[Size.x, Size.y, Size.z];

        meshFilter = GetComponent<MeshFilter>();

        meshCollider = GetComponent<MeshCollider>();


        cubes[Size.x / 2, Size.x / 2, Size.x / 2] = 1;
        transform.Translate(-Size / 2);


        createMesh();

    }
    void edit(Vector3Int pos)
    {
        if (pos.x > 0 && pos.x < Size.x - 1 && pos.y > 0 && pos.y < Size.y - 1 && pos.z > 0 && pos.z < Size.z - 1)
        {
            if (add)
            {
                cubes[pos.x, pos.y, pos.z] = 1;
            }
            else
            {
                cubes[pos.x, pos.y, pos.z] = 0;
            }
        }
        createMesh();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            add = false;
        }
        else
        {
            add = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (add)
                {
                    hit.normal = hit.normal * -1;
                }
                hit.point = hit.point - transform.position;
                //right
                if (hit.normal == Vector3.right)
                {
                    edit(new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z)));
                }
                //left
                if (hit.normal == Vector3.left)
                {
                    edit(new Vector3Int(Mathf.CeilToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z)));
                }
                //up
                if (hit.normal == Vector3.up)
                {
                    edit(new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.RoundToInt(hit.point.z)));
                }
                //down
                if (hit.normal == Vector3.down)
                {
                    edit(new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.CeilToInt(hit.point.y), Mathf.RoundToInt(hit.point.z)));
                }
                //forward
                if (hit.normal == Vector3.forward)
                {
                    edit(new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.FloorToInt(hit.point.z)));
                }
                //back
                if (hit.normal == Vector3.back)
                {
                    edit(new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.CeilToInt(hit.point.z)));
                }
            }
        }
    }
    void createFace(Vector3 normal)
    {
        int[] trisA ={
            index+0,index+1,index+2,
            index+2,index+1,index+3
        };
        Vector3[] normalsA ={
            normal,
            normal,
            normal,
            normal
        };
        Vector2[] uvsA ={
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(0,1)
        };

        tris.AddRange(trisA);
        normals.AddRange(normalsA);
        uvs.AddRange(uvsA);
        index += 4;
    }
    public void createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> vertices = new List<Vector3>();

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                for (int z = 0; z < Size.z; z++)
                {
                    if (cubes[x, y, z] != 0)
                    {

                        //right
                        bool right = false;
                        if (x != Size.x - 1)
                        {
                            if (cubes[x + 1, y, z] == 0)
                            {
                                right = true;
                            }
                        }
                        else
                        {
                            right = true;
                        }

                        //left
                        bool left = false;
                        if (x != 0)
                        {
                            if (cubes[x - 1, y, z] == 0)
                            {
                                left = true;
                            }
                        }
                        else
                        {
                            left = true;
                        }

                        //up
                        bool up = false;
                        if (y != Size.y - 1)
                        {
                            if (cubes[x, y + 1, z] == 0)
                            {
                                up = true;
                            }
                        }
                        else
                        {
                            up = true;
                        }

                        //down
                        bool down = false;
                        if (y != 0)
                        {
                            if (cubes[x, y - 1, z] == 0)
                            {
                                down = true;
                            }
                        }
                        else
                        {
                            down = true;
                        }

                        //forward
                        bool forward = false;
                        if (z != Size.z - 1)
                        {
                            if (cubes[x, y, z + 1] == 0)
                            {
                                forward = true;
                            }
                        }
                        else
                        {
                            forward = true;
                        }

                        //back
                        bool back = false;
                        if (z != 0)
                        {
                            if (cubes[x, y, z - 1] == 0)
                            {
                                back = true;
                            }
                        }
                        else
                        {
                            back = true;
                        }


                        if (right)
                        {
                            Vector3[] verticesA ={
                                new Vector3(x+0.5f,y+0.5f,z-0.5f),
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x+0.5f,y-0.5f,z-0.5f),
                                new Vector3(x+0.5f,y-0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.right);
                        }

                        if (left)
                        {
                            Vector3[] verticesA ={
                                new Vector3(x-0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z-0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z-0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.left);
                        }

                        if (up)
                        {
                            Vector3[] verticesA ={
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x+0.5f,y+0.5f,z-0.5f),
                                new Vector3(x-0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z-0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.up);
                        }

                        if (down)
                        {
                            Vector3[] verticesA ={
                                new Vector3(x+0.5f,y-0.5f,z-0.5f),
                                new Vector3(x+0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z-0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.down);
                        }

                        if (forward)
                        {
                            Vector3[] verticesA ={
                                new Vector3(x+0.5f,y-0.5f,z+0.5f),
                                new Vector3(x+0.5f,y+0.5f,z+0.5f),
                                new Vector3(x-0.5f,y-0.5f,z+0.5f),
                                new Vector3(x-0.5f,y+0.5f,z+0.5f)
                            };
                            vertices.AddRange(verticesA);
                            createFace(Vector3.forward);
                        }


                        if (back)
                        {
                            Vector3[] verticesA ={
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

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateTangents();

        index = 0;
        tris.Clear();
        normals.Clear();
        uvs.Clear();

    }
}
