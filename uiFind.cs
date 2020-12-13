using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class uiFind : MonoBehaviour
{
    [SerializeField]
    private Sprite arrow,point;
    bool view=false;
    public Transform target;
    //public Transform arrow;
    //public Transform point;
    [SerializeField]
    private float margin=60;
    private Vector2 screeenSize=new Vector2(Screen.width,Screen.height);
    private float rate=0;
    private Image image;
    Vector2 half;
    void Awake(){
        //margin=margin*0.002f*screeenSize.y;
        image=GetComponent<Image>();
        image.sprite=arrow;
        Vector2 a=new Vector2(screeenSize.y-margin*2,screeenSize.x-margin*2);
        a.Normalize();
        rate=a.y;
        half=new Vector2(screeenSize.x*0.5f,screeenSize.y*0.5f);
    }

    void Update()
    {
        
        Vector3 pos=Camera.main.WorldToScreenPoint(target.position);
        if(pos.z>0){
            if(!view){
                view=true;
                image.sprite=point;
                transform.rotation=Quaternion.identity;
            }
            if(pos.x<screeenSize.x&&pos.x>0&&pos.y<screeenSize.y&&pos.y>0){
                transform.position=pos;
            }else{
                if(view){
                    view=false;
                    image.sprite=arrow;
                }
                Vector2 normal=new Vector2(pos.x,pos.y)-half;
                normal.Normalize();
                bool value=Mathf.Abs(normal.x)<rate;
                if(value){
                    if(normal.y<0){
                        transform.position=new Vector2((-(normal.x/normal.y)*(half.y-margin))+half.x,margin);
                    }else{
                        transform.position=new Vector2(((normal.x/normal.y)*(half.y-margin))+half.x,screeenSize.y-margin);
                    }
                }else{
                    if(normal.x<0){
                        transform.position=new Vector2(margin,(-(normal.y/normal.x)*(half.x-margin))+half.y);
                    }else{
                        transform.position=new Vector2(screeenSize.x-margin,((normal.y/normal.x)*(half.x-margin))+half.y);
                    }
                }
                transform.localEulerAngles=Vector3.forward*Vector2.SignedAngle(Vector2.up,normal);
            }
        }else{
            if(view){
                view=false;
                image.sprite=arrow;
            }
            Vector2 normal=new Vector2(pos.x,pos.y)-half;
            normal.Normalize();
            bool value=Mathf.Abs(normal.x)<rate;
            if(value){
                if(normal.y>0){
                    transform.position=new Vector2((-(normal.x/normal.y)*(half.y-margin))+half.x,margin);
                }else{
                    transform.position=new Vector2(((normal.x/normal.y)*(half.y-margin))+half.x,screeenSize.y-margin);
                }
            }else{
                if(normal.x>0){
                    transform.position=new Vector2(margin,(-(normal.y/normal.x)*(half.x-margin))+half.y);
                }else{
                    transform.position=new Vector2(screeenSize.x-margin,((normal.y/normal.x)*(half.x-margin))+half.y);
                }
            }
            transform.localEulerAngles=Vector3.forward*Vector2.SignedAngle(Vector2.down,normal);
        }
    }
}
