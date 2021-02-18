using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class curveWay : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    Vector3[] pointsV;
    public float speed = 2;
    public bool look;
    float[] pointsLegenth;
    float legenthQuality = 0.01f;
    float timer = 0;
    int index = 0;
    bool active = true, returning = false;
    Vector3 retunPose;
#if UNITY_EDITOR
    int drawQuality = 20;
    public Color32 curveColor = Color.green, pointColor = Color.red;
    Color32 oldPointcolor = Color.red;

#endif
    private void Awake()
    {
        pointsV = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            pointsV[i] = points[i].position;
            Destroy(points[i].gameObject);
        }
        points.Clear();
    }
    public void activation(bool yes)
    {
        active = yes;
        if (yes)
        {
            timer = 0;
            retunPose = transform.position;
            returning = true;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < pointsV.Length; i++)
            {
                float distance = Vector3.Distance(retunPose, pointsV[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
        }


    }
    void Update()
    {
        if (active)
        {
            move();
        }

    }
    void move()
    {
        int oldIndex = index - 1;
        oldIndex = pointsV.Length + oldIndex;
        oldIndex = oldIndex % pointsV.Length;
        int newIndex = index + 1;
        newIndex = pointsV.Length + newIndex;
        newIndex = newIndex % pointsV.Length;

        Vector3 oldV = Vector3.Lerp(pointsV[oldIndex], pointsV[index], 0.5f);
        Vector3 newV = Vector3.Lerp(pointsV[index], pointsV[newIndex], 0.5f);

        oldV = returning ? retunPose : oldV;

        Vector3 fA = Vector3.Lerp(oldV, pointsV[index], timer);
        Vector3 fB = Vector3.Lerp(pointsV[index], newV, timer);
        Vector3 pos = Vector3.Lerp(fA, fB, timer);

        transform.position = pos;

        float last = timer + legenthQuality;
        Vector3 lA = Vector3.Lerp(oldV, pointsV[index], last);
        Vector3 lB = Vector3.Lerp(pointsV[index], newV, last);
        Vector3 drawEnd = Vector3.Lerp(lA, lB, last);
        if (look)
        {
            transform.rotation = Quaternion.LookRotation(pos - drawEnd, Vector3.up);
        }

        float movedDistance = Vector3.Distance(pos, drawEnd);

        timer += (speed * Time.deltaTime * ((legenthQuality / movedDistance) + 0.001f));
        if (timer > 1)
        {
            returning = false;
            timer = 0;
            index = (index == pointsV.Length - 1) ? 0 : index + 1;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int a = 0; a < points.Count; a++)
        {
            int oldIndex = a - 1;
            oldIndex = points.Count + oldIndex;
            oldIndex = oldIndex % points.Count;
            int newIndex = a + 1;
            newIndex = points.Count + newIndex;
            newIndex = newIndex % points.Count;

            Vector3 oldV = Vector3.Lerp(points[oldIndex].position, points[a].position, 0.5f);
            Vector3 newV = Vector3.Lerp(points[a].position, points[newIndex].position, 0.5f);

            for (int b = 0; b < drawQuality; b++)
            {
                float first = (1f / drawQuality) * b;
                Vector3 fA = Vector3.Lerp(oldV, points[a].position, first);
                Vector3 fB = Vector3.Lerp(points[a].position, newV, first);
                Vector3 drawStart = Vector3.Lerp(fA, fB, first);

                float last = (1f / drawQuality) * (b + 1);
                Vector3 lA = Vector3.Lerp(oldV, points[a].position, last);
                Vector3 lB = Vector3.Lerp(points[a].position, newV, last);
                Vector3 drawEnd = Vector3.Lerp(lA, lB, last);

                Gizmos.color = curveColor;
                Gizmos.DrawLine(drawStart, drawEnd);

            }
        }
        if (!pointColor.Equals(oldPointcolor))
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].gameObject.GetComponent<gizmo>().color = pointColor;
            }
            oldPointcolor = pointColor;
        }
    }

    public void addPoint()
    {
        Transform point = new GameObject().transform;
        points.Add(point);
        point.SetParent(transform);
        point.localPosition = Vector3.zero;
        point.gameObject.AddComponent<gizmo>();
        point.gameObject.name = (points.Count).ToString();
        Selection.activeGameObject = point.gameObject;
    }
    public void removePoint()
    {
        Transform active = Selection.activeTransform;
        if (points.Contains(active))
        {
            points.Remove(active);
            DestroyImmediate(active.gameObject);
        }

    }

#endif
}
#if UNITY_EDITOR
public class gizmo : MonoBehaviour
{
    public Color32 color = Color.red;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}

[CustomEditor(typeof(curveWay))]
public class addWay : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (!Application.isPlaying)
        {
            curveWay myway = (curveWay)target;
            if (GUILayout.Button("add"))
            {
                myway.addPoint();
            }
            if (GUILayout.Button("remove"))
            {
                myway.removePoint();
            }
        }


    }
}
#endif
