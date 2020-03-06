using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 是否需要发射 : MonoBehaviour
{
    串口 serialfood;
    public int sun;
    // Start is called before the first frame update
    void Start()
    {
        serialfood = GameObject.Find("食物的射击状态").GetComponent<串口>();
        float m = this.transform.eulerAngles.z;
        if (m >= 0)
        {
            float m1 =Mathf.Round( (m + 40) / 45);
            sun = (int)m1;

        }
        if (m < 0)
        {
            float m1 = Mathf.Round((Mathf.Abs( m) + 40) / 45);
            sun = (int)m1;
            if (sun != 1)
            {
                sun = 10 - sun;
            }
            if (sun == 1)
            {
                sun = 1;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (serialfood.mode[sun-1] == "按下开始了")
        {
            GameObject liziprefab = Resources.Load("发射粒子") as GameObject;
            GameObject lizi;
            Quaternion rotate = Quaternion.Euler(new Vector3(0, 0, 0));
            lizi = Instantiate(liziprefab, this.gameObject.transform.position, rotate) as GameObject;
            lizi.transform.parent = GameObject.Find("粒子特效集合").transform;
            lizi.GetComponent<贝塞尔曲线>().end = GameObject.Find("太阳位置/太阳" + (sun.ToString())).transform;
        }

    }
}
