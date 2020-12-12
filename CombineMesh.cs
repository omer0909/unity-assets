using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
{
    void Start()
    {
        Vector3 oldPosition=transform.position;
        Quaternion oldRotation=transform.rotation;
        transform.position=Vector3.zero;
        transform.rotation=Quaternion.identity;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
            
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        transform.position=oldPosition;
        transform.rotation=oldRotation;
    }
}
