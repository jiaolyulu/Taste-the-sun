using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 存在信息 : MonoBehaviour
{
    public int N;
    public bool [] sun;
    public bool[] purple_maisui;
    // Start is called before the first frame update
    void Start()
    {
        sun = new bool[8] { true,true, true, true, true, true, true, true};
        purple_maisui = new bool[8] { false, false, false, false, false, false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        int N_sun = 0;
        
        for (int j = 1; j <= 8; j++)
        {
            if (sun[j - 1] == true)
            {
                N_sun += 1;
                

            }
            else
            {
                if (purple_maisui [j-1]== false)
                {
                    GameObject Maisuiprefab = Resources.Load("麦穗2") as GameObject;
                    GameObject MaiSui;
                    float rotation = 0;
                    if (j <= 5)
                    {
                        rotation = 45 * (j - 1);
                    }
                    else
                    {
                        rotation = -45 * (9 - j);
                    }
                    Quaternion rotate = Quaternion.Euler(new Vector3(0, 0, rotation));
                    MaiSui = Instantiate(Maisuiprefab, new Vector3(0, 0, 0), rotate) as GameObject;
                    float m = Random.Range(2.6f, 3f);
                    MaiSui.transform.GetChild(0).gameObject.transform.localScale = new Vector3(m, m, m);
                    MaiSui.transform.parent = GameObject.Find("Canvas/麦穗中点").gameObject.transform;
                    MaiSui.transform.localScale = new Vector3(1, 1, 1);
                    MaiSui.transform.SetSiblingIndex(0);
                    purple_maisui[j - 1] = true;
                   
                }
                for (int p = 0; p <= GameObject.Find("Canvas/麦穗中点").transform.childCount - 1; p++)
                {
                    GameObject maisuixuanzhuan = GameObject.Find("Canvas/麦穗中点").transform.GetChild(p).gameObject;
                    if (!(maisuixuanzhuan.name.Contains("麦穗2")))
                    {
                        float angle_p = maisuixuanzhuan.transform.eulerAngles.z;
                        int dijigetaiyang = district(angle_p);
                        maisuixuanzhuan.GetComponent<麦穗的自我惠美>().dijigetaiyanga = dijigetaiyang;

                        if (sun[dijigetaiyang - 1] == true)
                        {
                            maisuixuanzhuan.GetComponent<麦穗的自我惠美>().startdestroy = true; ;

                        }
                    }
                    
                }

            }
        }
        
        N = N_sun;
    }
    public int district(float angle)
    {
        int k = 1;
        if (((angle > 0) & (angle < 22.5)) || ((angle < 0) & (angle > -22.5)))
        {
            k = 1;
        }
        if ((angle>=22.5)&(angle<67.5))
        {
            k = 2;
        }
        if ((angle >= 67.5) & (angle < 112.5))
        {
            k = 3;
        }
        if ((angle >= 112.5) & (angle < 157.5))
        {
            k = 4;
        }
        if (((angle >= 157.5) & (angle <=180))||((angle <= -157.5) & (angle >= -180)))
        {
            k = 5;
        }
        if ((angle > -157.5) & (angle <= -112.5))
        {
            k = 6;
        }
        if ((angle > -112.5) & (angle <= -67.5))
        {
            k = 7;
        }
        if ((angle > -67.5) & (angle <= -22.5))
        {
            k = 8;
        }
        return k;
    }
}
