using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class door : MonoBehaviour
{
    public KeyCode doorKey;
    public UnityEvent action;
    private bool inDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inDoor = true;
        }
        if (doorKey == KeyCode.None)
        {
            action.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inDoor = false;
        }
        if (doorKey == KeyCode.None)
        {
            action.Invoke();
        }
    }
    private void Update()
    {
        if (inDoor && Input.GetKeyDown(doorKey))
        {
            action.Invoke();
        }
    }
}
