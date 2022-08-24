using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CombineMesh : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public GameObject[] editedObjects = null;
    [SerializeField, HideInInspector]
    Vector3[] positions;
    [SerializeField, HideInInspector]
    Quaternion[] rotations;

    int GetChunk(GameObject[] chunks, GameObject chunkMesh)
    {
        float min = float.MaxValue;
        int minIndex = 0;
        Vector3 pos = chunkMesh.transform.position;
        for (int i = 0; i < chunks.Length; i++)
        {
            float distance = Vector3.Distance(pos, chunks[i].transform.position);
            if (distance < min)
            {
                min = distance;
                minIndex = i;
            }
        }
        return minIndex;
    }

    public void Clear()
    {
        for (int i = 0; i < editedObjects.Length; i++)
        {
            editedObjects[i].SetActive(true);
        }
        editedObjects = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject chunk = transform.GetChild(i).gameObject;
            DestroyImmediate(chunk.GetComponent<MeshFilter>());
            DestroyImmediate(chunk.GetComponent<MeshRenderer>());
            chunk.transform.position = positions[i];
            chunk.transform.rotation = rotations[i];
        }
    }

    public void CombineButton()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        GameObject[] chunks = new GameObject[transform.childCount];
        List<MeshFilter>[] chunkMeshes = new List<MeshFilter>[transform.childCount];
        positions = new Vector3[transform.childCount];
        rotations = new Quaternion[transform.childCount];
        List<GameObject> editedList = new List<GameObject>();

        for (int i = 0; i < chunkMeshes.Length; i++)
            chunkMeshes[i] = new List<MeshFilter>();

        for (int i = 0; i < transform.childCount; i++)
        {
            chunks[i] = transform.GetChild(i).gameObject;
            positions[i] = chunks[i].transform.position;
            rotations[i] = chunks[i].transform.rotation;
        }
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (!allObjects[i].isStatic || allObjects[i].GetComponent<MeshFilter>() == null)
                continue;
            chunkMeshes[GetChunk(chunks, allObjects[i])].Add(allObjects[i].GetComponent<MeshFilter>());
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            for (int j = 0; j < chunkMeshes[i].Count; j++)
                editedList.Add(chunkMeshes[i][j].gameObject);
            if (!Combine(chunks[i].gameObject, chunkMeshes[i].ToArray()))
            {
                editedObjects = editedList.ToArray();
                Clear();
                return;
            }
        }
        editedObjects = editedList.ToArray();
    }

    public bool Combine(GameObject chunk, MeshFilter[] meshFilters)
    {
        chunk.transform.position = Vector3.zero;
        chunk.transform.rotation = Quaternion.identity;

        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                Debug.LogWarning("\"" + meshRenderer.gameObject.name + "\" warning material length!");
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                if (meshRenderer.sharedMaterials[s] == null)
                {
                    Debug.LogError("\"" + meshRenderer.gameObject.name + "\" have null material!");
                    return false;
                }
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].GetHashCode());
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        MeshFilter meshFilterCombine = chunk.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
            meshFilterCombine = chunk.AddComponent<MeshFilter>();

        MeshRenderer meshRendererCombine = chunk.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
            meshRendererCombine = chunk.AddComponent<MeshRenderer>();

        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        foreach (MeshFilter meshFilter in meshFilters)
            meshFilter.gameObject.SetActive(false);
        return true;
    }

    private int Contains(ArrayList searchList, int searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).GetHashCode() == searchName)
            {
                return i;
            }
        }
        return -1;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CombineMesh))]
public class editMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CombineMesh edit = (CombineMesh)target;
        if (edit.editedObjects == null)
        {
            if (GUILayout.Button("combine"))
            {
                edit.CombineButton();
            }
        }
        else
        {
            if (GUILayout.Button("clear"))
            {
                edit.Clear();
            }
            if (GUILayout.Button("lightmap UV generate"))
            {
                for (int i = 0; i < edit.transform.childCount; i++)
                    Unwrapping.GenerateSecondaryUVSet(edit.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh);
            }
        }
    }

}
#endif
