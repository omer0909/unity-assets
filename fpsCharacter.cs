using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class fpsCharacter : MonoBehaviour
{
    private float velocity=0;
    private bool ground=false;
    private bool ceiling=false;
    public float jumpMultiply=5;
    public bool invertY=false;
     public float walkSpeed=5;
     public float runSpeed=10;
     public float cameraSensivity=3;
     private CharacterController charachter;
     private Transform fpsCamera;
     public Vector2 moveInput=Vector2.zero;
     public Vector2 cameraInput=Vector2.zero;
     private void Awake() {

        charachter=GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
        fpsCamera=Camera.main.transform;
        charachter.minMoveDistance=0;
     }
    private void cameraDirection(){
        Vector2 cameraOld=fpsCamera.eulerAngles;
        if(invertY){
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x+cameraInput.y*cameraSensivity),-90,90), cameraOld.y+cameraInput.x*cameraSensivity);
        }else{
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x-cameraInput.y*cameraSensivity),-90,90), cameraOld.y+cameraInput.x*cameraSensivity);
        }
    }
    void isInput(){
        cameraInput=new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));

        if(Input.GetKey(KeyCode.W)&&Input.GetKey(KeyCode.S)){
            moveInput.y=0;
        }else if(Input.GetKey(KeyCode.W)){
            moveInput.y=1;
        }else if(Input.GetKey(KeyCode.S)){
            moveInput.y=-1;
        }else{
            moveInput.y=0;
        }

        if(Input.GetKey(KeyCode.D)&&Input.GetKey(KeyCode.A)){
            moveInput.x=0;
        }else if(Input.GetKey(KeyCode.D)){
            moveInput.x=1;
        }else if(Input.GetKey(KeyCode.A)){
            moveInput.x=-1;
        }else{
            moveInput.x=0;
        }
    }
    void physics(){

        if(ground&&velocity<0){
            velocity=0;
        }
        if(ceiling&&velocity>0){
            velocity=0;
        }
        velocity-=Time.deltaTime*10;
    }

    private void move(){

        float speed=(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))? runSpeed: walkSpeed;
        Vector2 direction=new Vector2(Mathf.Sin(fpsCamera.eulerAngles.y*Mathf.Deg2Rad),Mathf.Cos(fpsCamera.eulerAngles.y*Mathf.Deg2Rad));
        Vector2 control=(1<moveInput.magnitude)?moveInput.normalized:moveInput;

        Vector2 x=control.y*direction;
        Vector2 y=-control.x*new Vector2(-direction.y,direction.x);
        Vector2 go=(x+y)*speed;
        Vector2 horizontalV=new Vector2(charachter.velocity.x,charachter.velocity.z);

        if(ground&&velocity<=0){
            go=Vector2.MoveTowards(horizontalV,go,30*Time.deltaTime);
            charachter.Move(new Vector3(go.x,-go.magnitude-0.1f,go.y)*Time.deltaTime);
        }else{
            go=Vector2.MoveTowards(horizontalV,go,5*Time.deltaTime);
            charachter.Move(new Vector3(go.x,velocity,go.y)*Time.deltaTime);
        }
    }
    void jump(){
        if(Input.GetKeyDown(KeyCode.Space)&&ground){
            velocity=jumpMultiply;
        }
    }
    
    private void isGround(){
        ground=charachter.isGrounded;
        RaycastHit hit;
        ceiling=(Physics.Raycast( transform.position,Vector3.up,out hit,0.1f+charachter.height*0.5f));
    }
    void Update()
    {
        isInput();
        isGround();
        cameraDirection();
        jump();
        physics();
        move();
    }
}
