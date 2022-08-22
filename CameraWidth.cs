using UnityEngine;
[RequireComponent(typeof(Camera))]

[ExecuteInEditMode]
public class CameraWidth : MonoBehaviour
{
    public float rateView = 25;
    float multiplay;
    void Awake()
    {
        CamUpdate();
    }

    void CamUpdate()
    {
        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            cam.orthographicSize = rateView * (1 / cam.aspect);
        }
        else
        {
            float angle = Mathf.Tan(rateView * 0.5f * Mathf.Deg2Rad);
            angle *= 1 / cam.aspect;
            cam.fieldOfView = Mathf.Atan(angle) * Mathf.Rad2Deg * 2;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        CamUpdate();
    }
#endif
}
