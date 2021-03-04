using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class door : MonoBehaviour
{
    public KeyCode doorKey;
    public UnityEvent action;
    public int inDoor;
    private void OnTriggerEnter(Collider other)
    {
        inDoor++;
        if (inDoor == 1 && doorKey == KeyCode.None)
        {
            action.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        inDoor--;
        if (inDoor == 0 && doorKey == KeyCode.None)
        {
            action.Invoke();
        }
    }
    private void Update()
    {
        if (inDoor > 0 && Input.GetKeyDown(doorKey))
        {
            action.Invoke();
        }
    }
}
