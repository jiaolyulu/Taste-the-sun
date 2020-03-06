using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 麦穗的生长控制脚本 : MonoBehaviour
{
    public int add;
    public int N_sun;
    存在信息 sun_existed;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        sun_existed = GameObject.Find("太阳的存在状态").GetComponent<存在信息>();
    }

    // Update is called once per frame
    void Update()
    {
        N_sun = sun_existed.N;
        i++;
        if (i >= N_sun*12)
        {
            add++;
            i = 0;

        }
    }
}
