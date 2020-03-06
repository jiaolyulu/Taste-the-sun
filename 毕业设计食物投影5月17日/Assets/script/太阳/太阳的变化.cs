using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 太阳的变化 : MonoBehaviour
{
    存在信息 taiyang;
    bool b_sun;
    int i;
    bool pre;
    GameObject liziforcefield;
    GameObject bluetree;
    Animator bluetree_ani;
    // Start is called before the first frame update
    void Start()
    {
        taiyang = GameObject.Find("太阳的存在状态").GetComponent<存在信息>();
        i =int.Parse( this.name.Substring(2,1))-1;
        liziforcefield = this.gameObject.transform.GetChild(1).gameObject;
        bluetree = GameObject.Find("Canvas/蓝色背景遮罩/太阳" + (i + 1).ToString() + "/蓝色树");
        bluetree_ani = bluetree.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool b_sun = taiyang.sun[i];
        if ((b_sun==true)&(pre==false))
        {
            liziforcefield.GetComponent<ParticleSystemForceField>().endRange = 156;
            bluetree_ani.SetBool("chuxian",true);
            bluetree_ani.SetBool("xiaoshi", false);
        }
        if ((b_sun==false)&(pre==true))
        {
            liziforcefield.GetComponent<ParticleSystemForceField>().endRange = 56;
            bluetree_ani.SetBool("xiaoshi", true);
            bluetree_ani.SetBool("chuxian", false);
        }
        pre = b_sun;
    }
}
