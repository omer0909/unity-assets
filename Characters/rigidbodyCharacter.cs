using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
//Add Physics Material and set friction 0
//Rigidbody set drag 4 and set interpolate interpolate and constraints rotations
public class rigidbodyCharacter : MonoBehaviour
{
    public bool invertY = false;
    public float jumpMultiply = 300, walkSpeed = 30, runSpeed = 60, cameraSensivity = 3, stopDrag = 10;
    private Rigidbody r;
    private Transform fpsCamera;
    private Vector2 moveInput, cameraInput, input;
    private bool jump, isGround;
    private Vector3 ground = Vector3.up;
    private float drag;
    private void Awake()
    {
        r = GetComponent<Rigidbody>();
        fpsCamera = Camera.main.transform;
        drag = r.drag;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void cameraDirection()
    {
        Vector2 cameraOld = fpsCamera.eulerAngles;
        if (invertY)
        {
            fpsCamera.eulerAngles = new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0, cameraOld.x + cameraInput.y * cameraSensivity), -90, 90), cameraOld.y + cameraInput.x * cameraSensivity);
        }
        else
        {
            fpsCamera.eulerAngles = new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0, cameraOld.x - cameraInput.y * cameraSensivity), -90, 90), cameraOld.y + cameraInput.x * cameraSensivity);
        }
    }
    void isInput()
    {
        cameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            moveInput.y = 0;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            moveInput.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveInput.y = -1;
        }
        else
        {
            moveInput.y = 0;
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            moveInput.x = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveInput.x = -1;
        }
        else
        {
            moveInput.x = 0;
        }
    }
    void Update()
    {
        cameraDirection();
        isInput();
    }
    private void OnCollisionStay(Collision other)
    {

        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector3.Angle(Vector3.up, contact.normal) < 45)
            {
                isGround = true;
                ground = contact.normal;
            }
        }
    }
    private void FixedUpdate()
    {
        r.useGravity = !isGround;

        float speed = (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector2 direction = new Vector2(Mathf.Sin(fpsCamera.eulerAngles.y * Mathf.Deg2Rad), Mathf.Cos(fpsCamera.eulerAngles.y * Mathf.Deg2Rad));
        Vector2 control = Vector2.ClampMagnitude(moveInput, 1);

        Vector2 x = control.y * direction;
        Vector2 y = -control.x * new Vector2(-direction.y, direction.x);
        Vector2 go = (x + y) * speed;

        if (isGround)
        {
            r.drag = moveInput == Vector2.zero ? stopDrag : drag;

            Quaternion moveDirection = Quaternion.FromToRotation(ground, Vector3.up);
            Vector3 move = moveDirection * new Vector3(go.x, 0, go.y);
            move = (move.y < 0) ? new Vector3(go.x * go.x / move.x, 0, go.y * go.y / move.z) : new Vector3(move.x, -move.y, move.z);
            r.AddForce(move);
        }
        else
        {
            r.drag = 0.5f;
            r.AddForce(new Vector3(go.x * 0.3f / drag, 0, go.y * 0.3f / drag));
        }

        if (jump)
        {
            jump = false;
            if (isGround)
                r.AddForce(jumpMultiply * Vector3.up);
        }

        isGround = false;
        r.AddForce(0, -0.001f, 0);
    }
}
