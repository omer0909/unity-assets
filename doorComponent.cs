using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorComponent : MonoBehaviour
{
    private bool open = false;
    public Vector3 pos, angle, scale;
    private Vector3 mPos, mAngle, mScale;
    public AnimationCurve curve;
    public float time = 1;
    private float timer;
    private void Awake()
    {
        mPos = transform.localPosition;
        mAngle = transform.localEulerAngles;
        mScale = transform.localScale;

        if (curve.length == 0)
        {
            curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            curve.preWrapMode = WrapMode.PingPong;
            curve.postWrapMode = WrapMode.PingPong;
        }
        timer = open ? 1 : 0;
    }
    public void click()
    {
        open = !open;
    }
    private void move()
    {
        float value = curve.Evaluate(timer);
        transform.localPosition = Vector3.Lerp(mPos, mPos + pos, value);
        transform.localEulerAngles = Vector3.Lerp(mAngle, mAngle + angle, value);
        transform.localScale = Vector3.Lerp(mScale, mScale + scale, value);
    }
    private void Update()
    {
        int value = open ? 1 : 0;
        if (!Mathf.Approximately(timer, value))
        {
            timer = Mathf.MoveTowards(timer, value, Time.deltaTime / time);
            move();
        }
    }
}
