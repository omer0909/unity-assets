using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class fpsCharacter : MonoBehaviour
{
    
    private bool ground=false;
    [SerializeField]
    private AnimationCurve jumpForce;
    public float jumpMultiply=5;
    public bool invertY=false;
     public float walkSpeed=5;
     public float runSpeed=10;
     public float cameraSensivity=3;
     private CharacterController charachter;
     private Transform fpsCamera;
     private bool jumping=false;
     public Vector2 moveInput=Vector2.zero;
     private void Awake() {
         if (jumpForce.length==0){
            jumpForce = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
            jumpForce.preWrapMode = WrapMode.PingPong;
            jumpForce.postWrapMode = WrapMode.PingPong;
        }

        charachter=GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
        fpsCamera=Camera.main.transform;
        charachter.minMoveDistance=0;
     }
    private void cameraDirection(){
        Vector2 cameraOld=fpsCamera.eulerAngles;
        if(invertY){
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x+Input.GetAxis("Mouse Y")*cameraSensivity),-90,90), cameraOld.y+Input.GetAxis("Mouse X")*cameraSensivity);
        }else{
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x-Input.GetAxis("Mouse Y")*cameraSensivity),-90,90), cameraOld.y+Input.GetAxis("Mouse X")*cameraSensivity);
        }
    }
    void isInput(){
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

    private void move(){

        float speed=(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))? runSpeed: walkSpeed;
        Vector2 direction=new Vector2(Mathf.Sin(fpsCamera.eulerAngles.y*0.0174532925f),Mathf.Cos(fpsCamera.eulerAngles.y*0.0174532925f));
        Vector2 control=(1<moveInput.magnitude)?moveInput.normalized:moveInput;

        Vector2 x=control.y*direction;
        Vector2 y=-control.x*new Vector2(-direction.y,direction.x);
        Vector2 go=(x+y)*speed;
        Vector2 velocity=new Vector2(charachter.velocity.x,charachter.velocity.z);
        go=Vector2.MoveTowards(velocity,go,30*Time.deltaTime);

        if(ground){
            charachter.SimpleMove(new Vector3(go.x,-go.magnitude,go.y));
        }else{
            charachter.SimpleMove(new Vector3(go.x,0,go.y));
        }
    }
    void jump(){
        if(Input.GetKeyDown(KeyCode.Space)&&ground){
            StartCoroutine(JumpEvent());
        }
    }
    
    private IEnumerator JumpEvent(){
        float jumptime=0;
        bool jumpingup=false;
        charachter.slopeLimit=90;
        if(!jumping){
            jumping=true;
            do{
                if(ground==false&&jumpingup==false){
                    jumpingup=true;
                }else if(ground&&jumpingup){
                    break;
                }
                jumptime+=Time.deltaTime;
                charachter.Move(Vector3.up*jumpMultiply*jumpForce.Evaluate(jumptime)*Time.deltaTime);
                yield return null;
            }while(jumptime<1f);
        }
        charachter.slopeLimit=45;
        jumping=false;
    }
    
    private void isGround(){
        RaycastHit hit;
        ground=(charachter.isGrounded||Physics.Raycast( transform.position,Vector3.down,out hit,0.1f+charachter.height*0.5f));
    }
    void Update()
    {
        isGround();
        cameraDirection();
        isInput();
        move();
        jump();
    }
    }
