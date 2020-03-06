using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 麦穗是否需要扩散 : MonoBehaviour
{
    float r;
    public float anglerange;
    public Vector3 newpos;
    public float radias;
    public GameObject anchor, plant;
    public float speed;
    Vector3 vec1, vec2;
    public bool zuo, you;
    int i;
    public GameObject maisuizd;
    public float cha,chazuo,chayou;
    // Start is called before the first frame update
    void Start()
    {
        chazuo = -anglerange;
        chayou = anglerange;
        anchor = this.gameObject;
        plant = gameObject.transform.GetChild(0).gameObject;
        if (radias == 0)
        {
            radias = 1.0f;
        }
        newpos = plant.transform.position;
        vec1 = anchor.transform.position;
        vec2 = plant.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        r = this.transform.rotation.eulerAngles.z;
        zuo = false;
        you = false;
        
        maisuizd = GameObject.Find("Canvas/麦穗中点");            
        foreach (Transform child in GameObject.Find("Canvas/麦穗中点").transform)
        {
            cha = child.transform.eulerAngles.z - r;
            if (cha > 180)
            {
                cha = cha - 360;
            }
            if (cha < -180)
            {
                cha = cha + 360;
            }
            if ((cha < chayou) & (cha > 0))
            {
                you = true;
                chayou = cha;
            }
            if ((cha > chazuo) & (cha < 0))
            {
                zuo = true;
                chazuo = cha;
            }
            
        }
        if ((zuo == true) & (you == true))
        {
            radias = Random.RandomRange(1.1f, 1.3f);

        }
        if (this.transform.localScale.x < radias)
        {
            this.transform.localScale += new Vector3(Time.deltaTime,Time.deltaTime,Time.deltaTime);
        }
    }

    
}
