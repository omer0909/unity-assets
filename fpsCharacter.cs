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
     private void Awake() {
         if (jumpForce.length==0){
            jumpForce = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
            jumpForce.preWrapMode = WrapMode.PingPong;
            jumpForce.postWrapMode = WrapMode.PingPong;
        }

        charachter=GetComponent<CharacterController>();

        
         Cursor.visible=false;
         fpsCamera=Camera.main.transform;
         charachter.slopeLimit=90;
     }
    private void cameraDirection(){
        Vector2 cameraOld=fpsCamera.eulerAngles;
        if(invertY){
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x+Input.GetAxis("Mouse Y")*cameraSensivity),-90,90), cameraOld.y+Input.GetAxis("Mouse X")*cameraSensivity);
        }else{
            fpsCamera.eulerAngles=new Vector2(Mathf.Clamp(Mathf.DeltaAngle(0,cameraOld.x-Input.GetAxis("Mouse Y")*cameraSensivity),-90,90), cameraOld.y+Input.GetAxis("Mouse X")*cameraSensivity);
        }
    }

    private void move(){

        float speed=(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))? runSpeed: walkSpeed;
        Vector2 direction=new Vector2(Mathf.Sin(fpsCamera.eulerAngles.y*0.0174532925f),Mathf.Cos(fpsCamera.eulerAngles.y*0.0174532925f));
        Vector2 input=new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        Vector2 control=(1<input.magnitude)?input.normalized:input;

        Vector2 x=control.y*direction;
        Vector2 y=-control.x*new Vector2(-direction.y,direction.x);
        Vector3 go=(x+y)*speed;

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
        bool jumping=false;
        do{
            charachter.Move(Vector3.up*jumpMultiply*jumpForce.Evaluate(jumptime)*Time.deltaTime);
            jumptime+=Time.deltaTime;

            if(ground==false&&jumping==false){
                jumping=true;
            }else if(ground&&jumping){
                jumptime=1;
            }
            
            yield return null;
        }while(jumptime<1f);
    }
    
    private void isGround(){
        RaycastHit hit;
        ground=(charachter.isGrounded||Physics.Raycast( transform.position,Vector3.down,out hit,0.1f+charachter.height*0.5f));
    }
    void Update()
    {
        isGround();
        cameraDirection();
        move();
        jump();
    }
    }
