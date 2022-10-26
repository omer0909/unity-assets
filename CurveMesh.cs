using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CurveMesh : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    Vector3[] pointsV;
    public int drawQuality = 4;
    public bool cyclic = false;
#if UNITY_EDITOR
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
    public void GetPoint(float time, out Vector3 pos, out Vector3 dir)
    {
        float timer = 0;
        int count = cyclic ? pointsV.Length : pointsV.Length - 1;
        if (!cyclic)
        {
            Vector3 posA = pointsV[0];
            Vector3 posB = Vector3.Lerp(pointsV[0], pointsV[1], 0.5f);
            float distance = Vector3.Distance(posA, posB);
            if (time < 0)
            {
                dir = (posB - posA).normalized;
                pos = posA + dir * time;
                return;
            }
            if (time <= distance)
            {
                pos = Vector3.Lerp(posA, posB, time / distance);
                dir = (posB - posA).normalized;
                return;
            }
            timer += distance;
        }
        for (int a = cyclic ? 0 : 1; a < count; a++)
        {

            int oldIndex = a - 1;
            oldIndex = oldIndex % pointsV.Length;
            int newIndex = a + 1;
            newIndex = newIndex % pointsV.Length;

            Vector3 oldV = Vector3.Lerp(pointsV[oldIndex], pointsV[a], 0.5f);
            Vector3 newV = Vector3.Lerp(pointsV[a], pointsV[newIndex], 0.5f);
            for (int b = 0; b < drawQuality; b++)
            {
                float first = (1f / drawQuality) * b;
                Vector3 fA = Vector3.Lerp(oldV, pointsV[a], first);
                Vector3 fB = Vector3.Lerp(pointsV[a], newV, first);
                Vector3 drawStart = Vector3.Lerp(fA, fB, first);

                float last = (1f / drawQuality) * (b + 1);
                Vector3 lA = Vector3.Lerp(oldV, pointsV[a], last);
                Vector3 lB = Vector3.Lerp(pointsV[a], newV, last);
                Vector3 drawEnd = Vector3.Lerp(lA, lB, last);

                float distance = Vector3.Distance(drawStart, drawEnd);
                if (timer + distance >= time)
                {
                    pos = Vector3.Lerp(drawStart, drawEnd, (time - timer) / distance);
                    dir = (drawEnd - drawStart).normalized;
                    return;
                }
                timer += distance;
            }
        }
        if (!cyclic)
        {
            Vector3 posA = Vector3.Lerp(pointsV[count], pointsV[count - 1], 0.5f);
            Vector3 posB = pointsV[count];
            float distance = Vector3.Distance(posA, posB);
            if (time <= distance + timer)
            {
                pos = Vector3.Lerp(posA, posB, (time - timer) / distance);
                dir = (posB - posA).normalized;
                return;
            }
            timer += distance;
            dir = (posB - posA).normalized;
            pos = posB + dir * (time - timer);
            return;
        }
        pos = Vector3.zero;
        dir = Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        int count = cyclic ? points.Count : points.Count - 1;
        if (!cyclic)
        {
            Gizmos.color = curveColor;
            Gizmos.DrawLine(points[0].position, Vector3.Lerp(points[0].position, points[1].position, 0.5f));
            Gizmos.DrawLine(points[count].position, Vector3.Lerp(points[count].position, points[count - 1].position, 0.5f));
        }
        for (int a = cyclic ? 0 : 1; a < count; a++)
        {
            int oldIndex = a - 1;
            oldIndex = oldIndex % points.Count;
            int newIndex = a + 1;
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

    public void AddPoint()
    {
        Transform point = new GameObject().transform;
        points.Add(point);
        point.SetParent(transform);
        point.localPosition = Vector3.zero;
        point.gameObject.AddComponent<gizmo>();
        point.gameObject.name = (points.Count).ToString();
        Selection.activeGameObject = point.gameObject;
    }
    public void RemovePoint()
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

[CustomEditor(typeof(CurveMesh))]
public class addWay : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (!Application.isPlaying)
        {
            CurveMesh myway = (CurveMesh)target;
            if (GUILayout.Button("add"))
            {
                myway.AddPoint();
            }
            if (GUILayout.Button("remove"))
            {
                myway.RemovePoint();
            }
        }
    }
}
#endif