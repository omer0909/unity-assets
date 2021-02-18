using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class editMeshScript : MonoBehaviour

{
    public float margin=0.1f;
    private int count(){
        return gameObject.transform.GetChildCount();
    }
    public void snap(){
        for(int i=0;i < count();i++){
            Transform objectT= gameObject.transform.GetChild(i);
            Vector3 pos =objectT.localPosition;
            if(Mathf.Abs(Mathf.Round(pos.x)-pos.x)<margin){
                pos.x=Mathf.Round(pos.x);
            }
            if(Mathf.Abs(Mathf.Round(pos.y)-pos.y)<margin){
                pos.y=Mathf.Round(pos.y);
            }
            if(Mathf.Abs(Mathf.Round(pos.z)-pos.z)<margin){
                pos.z=Mathf.Round(pos.z);
            }
            objectT.localPosition=pos;
        }
    }
    public void hide(){
        for(int i=0;i < count();i++){
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void show(){
        
        for(int i=0;i < count();i++){
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void transformControl(Action function)
    {
        Vector3 oldPosition=transform.position;
        Quaternion oldRotation=transform.rotation;
        transform.position=Vector3.zero;
        transform.rotation=Quaternion.identity;
        function();
        transform.position=oldPosition;
        transform.rotation=oldRotation;
    }
    public void combineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }
    public void combineCollider(){
        MeshCollider[] meshColliders = GetComponentsInChildren<MeshCollider>();
        CombineInstance[] combine = new CombineInstance[meshColliders.Length];

        int i = 1;
        while (i < meshColliders.Length)
        {
            combine[i].mesh = meshColliders[i].sharedMesh;
            combine[i].transform = meshColliders[i].transform.localToWorldMatrix;

            i++;
        }
        transform.GetComponent<MeshCollider>().sharedMesh = new Mesh();
        //transform.GetComponent<MeshCollider>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        transform.GetComponent<MeshCollider>().sharedMesh.CombineMeshes(combine);
    }
    public MeshFilter uvUnwrap(){
        return transform.GetComponent<MeshFilter>();
    }
    public void addMaterial(){
        GetComponent<MeshRenderer>().material=gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(editMeshScript))]
public class editMapEditor : Editor
{
    public override void OnInspectorGUI(){
        DrawDefaultInspector();

        editMeshScript edit=(editMeshScript)target;
        if(GUILayout.Button("snap")){
            edit.snap();
        }
        if(GUILayout.Button("hide")){
            edit.hide();
        }
        if(GUILayout.Button("show")){
            edit.show();
        }
        GUILayout.Space(10);
        if(GUILayout.Button("combine")){
            edit.transformControl(edit.combineMesh);
        }
        if(GUILayout.Button("combine collider")){
            edit.transformControl(edit.combineCollider);
        }
        if(GUILayout.Button("add material")){
            edit.addMaterial();
        }
        if(GUILayout.Button("uv unwrap")){
            Unwrapping.GenerateSecondaryUVSet(edit.uvUnwrap().sharedMesh);
        }
    }

}
#endif
