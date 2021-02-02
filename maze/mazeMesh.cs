using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class mazeMesh : MonoBehaviour
{
    public int matrix = 29;
    bool[,] mazeWall;
    public Transform outPos;
    void Awake()
    {
        mazeWall = new bool[matrix, matrix];

        List<Vector2Int> way = new List<Vector2Int>();

        way.Add(Vector2Int.one);
        mazeWall[1, 1] = true;

        int index = 0;
        int maxIndex = 0;
        Vector2Int maxPos = Vector2Int.zero;

        while (true)
        {
            Vector2Int direction = control(way[index]);

            if (direction == Vector2Int.zero)
            {
                if (index > maxIndex)
                {
                    maxIndex = index;
                    maxPos = way[index];
                }

                index--;
                way.RemoveAt(way.Count - 1);
            }
            else
            {
                way.Add(way[index] + direction * 2);

                mazeWall[(way[index] + direction).x, (way[index] + direction).y] = true;
                mazeWall[(way[index] + direction * 2).x, (way[index] + direction * 2).y] = true;
                index++;
            }



            if (index == 0)
            {
                break;
            }
        }
        outPos.position = new Vector3(maxPos.x, 0, maxPos.y);

        Mesh newmesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> tris = new List<int>();

        int trisIndex = 0;

        for (int x = 0; x < matrix; x++)
        {
            for (int y = 0; y < matrix; y++)
            {

                if (!mazeWall[x, y])
                {
                    bool back = false, forward = false, left = false, right = false;

                    if (y + 1 < matrix)
                    {
                        if (mazeWall[x, y + 1])
                        {
                            back = true;
                        }
                    }
                    else
                    {
                        back = true;
                    }

                    if (y > 0)
                    {
                        if (mazeWall[x, y - 1])
                        {
                            forward = true;
                        }
                    }
                    else
                    {
                        forward = true;
                    }

                    if (x + 1 < matrix)
                    {
                        if (mazeWall[x + 1, y])
                        {
                            left = true;
                        }
                    }
                    else
                    {
                        left = true;
                    }

                    if (x > 0)
                    {
                        if (mazeWall[x - 1, y])
                        {
                            right = true;
                        }
                    }
                    else
                    {
                        right = true;
                    }

                    if (forward)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            normals.Add(Vector3.back);
                        }
                        Vector3[] PlaneV ={
                                new Vector3(x+0.5f,1,y-0.5f),
                                new Vector3(x-0.5f,1,y-0.5f),
                                new Vector3(x+0.5f,0,y-0.5f),
                                new Vector3(x-0.5f,0,y-0.5f)
                            };
                        vertices.AddRange(PlaneV);
                        int[] PlaneT ={
                                trisIndex,trisIndex+3,trisIndex+1,
                                trisIndex,trisIndex+2,trisIndex+3
                            };
                        tris.AddRange(PlaneT);
                        trisIndex += 4;
                        Vector2[] PlaneU ={
                                new Vector2(1,1),
                                new Vector2(0,1),
                                new Vector2(1,0),
                                new Vector2(0,0)
                            };
                        uv.AddRange(PlaneU);
                    }

                    if (back)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            normals.Add(Vector3.forward);
                        }
                        Vector3[] PlaneV ={
                                new Vector3(x+0.5f,1,y+0.5f),
                                new Vector3(x-0.5f,1,y+0.5f),
                                new Vector3(x+0.5f,0,y+0.5f),
                                new Vector3(x-0.5f,0,y+0.5f)
                            };
                        vertices.AddRange(PlaneV);
                        int[] PlaneT ={
                                trisIndex,trisIndex+1,trisIndex+3,
                                trisIndex,trisIndex+3,trisIndex+2
                            };
                        tris.AddRange(PlaneT);
                        trisIndex += 4;
                        Vector2[] PlaneU ={
                                new Vector2(1,1),
                                new Vector2(0,1),
                                new Vector2(1,0),
                                new Vector2(0,0)
                            };
                        uv.AddRange(PlaneU);
                    }

                    if (right)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            normals.Add(Vector3.left);
                        }
                        Vector3[] PlaneV ={
                                new Vector3(x-0.5f,1,y+0.5f),
                                new Vector3(x-0.5f,1,y-0.5f),
                                new Vector3(x-0.5f,0,y+0.5f),
                                new Vector3(x-0.5f,0,y-0.5f)
                            };
                        vertices.AddRange(PlaneV);
                        int[] PlaneT ={
                                trisIndex,trisIndex+1,trisIndex+3,
                                trisIndex,trisIndex+3,trisIndex+2
                            };
                        tris.AddRange(PlaneT);
                        trisIndex += 4;
                        Vector2[] PlaneU ={
                                new Vector2(1,1),
                                new Vector2(0,1),
                                new Vector2(1,0),
                                new Vector2(0,0)
                            };
                        uv.AddRange(PlaneU);
                    }

                    if (left)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            normals.Add(Vector3.right);
                        }
                        Vector3[] PlaneV ={
                                new Vector3(x+0.5f,1,y+0.5f),
                                new Vector3(x+0.5f,1,y-0.5f),
                                new Vector3(x+0.5f,0,y+0.5f),
                                new Vector3(x+0.5f,0,y-0.5f)
                            };
                        vertices.AddRange(PlaneV);
                        int[] PlaneT ={
                                trisIndex,trisIndex+3,trisIndex+1,
                                trisIndex,trisIndex+2,trisIndex+3
                            };
                        tris.AddRange(PlaneT);
                        trisIndex += 4;
                        Vector2[] PlaneU ={
                                new Vector2(1,1),
                                new Vector2(0,1),
                                new Vector2(1,0),
                                new Vector2(0,0)
                            };
                        uv.AddRange(PlaneU);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        normals.Add(Vector3.up);
                    }
                    Vector3[] PlaneVP ={
                        new Vector3(x+0.5f,1,y+0.5f),
                        new Vector3(x+0.5f,1,y-0.5f),
                        new Vector3(x-0.5f,1,y+0.5f),
                        new Vector3(x-0.5f,1,y-0.5f)
                    };
                    vertices.AddRange(PlaneVP);
                    int[] PlaneTP ={
                        trisIndex,trisIndex+1,trisIndex+3,
                        trisIndex,trisIndex+3,trisIndex+2
                    };
                    tris.AddRange(PlaneTP);
                    trisIndex += 4;
                    Vector2[] PlaneUP ={
                        new Vector2(1,1),
                        new Vector2(1,0),
                        new Vector2(0,1),
                        new Vector2(0,0)
                    };
                    uv.AddRange(PlaneUP);
                }
            }
        }
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.up);
        }
        Vector3[] PlaneVertex ={
            new Vector3(matrix-0.5f,0,matrix-0.5f),
            new Vector3(matrix-0.5f,0,-0.5f),
            new Vector3(-0.5f,0,matrix-0.5f),
            new Vector3(-0.5f,0,-0.5f)
        };
        vertices.AddRange(PlaneVertex);
        int[] PlaneTris ={
            trisIndex,trisIndex+1,trisIndex+3,
            trisIndex,trisIndex+3,trisIndex+2
        };
        tris.AddRange(PlaneTris);
        Vector2[] PlaneUv ={
            new Vector2(matrix,matrix),
            new Vector2(matrix,0),
            new Vector2(0,matrix),
            new Vector2(0,0)
        };
        uv.AddRange(PlaneUv);


        newmesh.vertices = vertices.ToArray();
        newmesh.normals = normals.ToArray();
        newmesh.triangles = tris.ToArray();
        newmesh.uv = uv.ToArray();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = newmesh;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = newmesh;

    }


    Vector2Int control(Vector2Int pos)
    {
        List<Vector2Int> direction = new List<Vector2Int>();
        if (pos.x + 2 < matrix)
        {
            if (!mazeWall[pos.x + 2, pos.y])
            {

                direction.Add(Vector2Int.right);
            }
        }

        if (pos.y + 2 < matrix)
        {
            if (!mazeWall[pos.x, pos.y + 2])
            {
                direction.Add(Vector2Int.up);
            }
        }

        if (pos.x - 1 > 0)
        {
            if (!mazeWall[pos.x - 2, pos.y])
            {
                direction.Add(Vector2Int.left);
            }
        }

        if (pos.y - 1 > 0)
        {
            if (!mazeWall[pos.x, pos.y - 2])
            {
                direction.Add(Vector2Int.down);
            }
        }
        Vector2Int result = Vector2Int.zero;
        if (direction.Count != 0)
        {
            result = direction[(int)Random.Range(0, direction.Count)];
        }
        return result;
    }
}
