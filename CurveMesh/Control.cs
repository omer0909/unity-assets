using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Control : MonoBehaviour
{
    public CurveMesh curve;
    private Mesh mesh;
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        EditMesh();
    }

    void EditMesh()
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (int v = 0; v < vertices.Length; v++)
        {
            Vector3 pos;
            Vector3 dir;
            Vector3 vertex = transform.TransformPoint(vertices[v]);
            curve.GetPoint(vertex.x, out pos, out dir);
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            Vector3 add = rotation * new Vector3(-vertex.z, vertex.y, 0);
            vertex = pos + add;
            vertices[v] = transform.InverseTransformPoint(vertex);
            normals[v] = rotation * new Vector3(-normals[v].z, normals[v].y, normals[v].x);
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
}
