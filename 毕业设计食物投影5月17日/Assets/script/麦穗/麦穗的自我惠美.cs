using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 麦穗的自我惠美 : MonoBehaviour
{
    int i=0;
    public 存在信息 sun;
    public Animator ani;
    int time;
    public int ik;
    public int istart;
    public bool startdestroy = false;
    public int dijigetaiyanga;
    bool isstarting = false;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    void Start()
    {
        ik = 0;
        ani = this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        i++;
        
        sun = GameObject.Find("太阳的存在状态").GetComponent<存在信息>();
        if (this.gameObject.name .Contains("麦穗2"))
        {
            float rotation = this.gameObject.transform.eulerAngles.z;
            float zhengchu = 0;
            
            if (rotation >= 0)
            {
                zhengchu = rotation / 45;
                ik = (int)zhengchu;
            }
            else
            {
                zhengchu = (-rotation) / 45;
                ik = (int)zhengchu;
                ik = 8 - ik;
            }


            if (sun.sun[ik] == true)
            {
                startdestroy = true;
                if (!isstarting)
                {
                    isstarting = true;
                    istart = i;
                }

            }
            
            if (startdestroy)
            {
                ani.SetBool("dieing", true);
                if (i >= (istart + 70))
                {
                    sun.purple_maisui[ik] = false;
                    Destroy(this.gameObject);
                }
            }
          

            



        }
        else
        {
           
            int n = sun.N;
            time = (25 - 3 * n) * 2;
           
            if (i >= time * 60)
            {
                startdestroy = true;
                if (!isstarting)
                {
                    isstarting = true;
                    istart = i;
                }
            }
            if (startdestroy)
            {
                if (i == istart)
                {
                    ani.SetBool("dieing", true);
                }

                if (i >= (istart + 110))
                {
                    Destroy(this.gameObject);
                }
            }
               
            
        }
        
       
        
    }

}
