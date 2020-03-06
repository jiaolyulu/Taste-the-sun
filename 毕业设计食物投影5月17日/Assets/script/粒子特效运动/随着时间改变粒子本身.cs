using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 随着时间改变粒子本身 : MonoBehaviour
{
    float starttime, endtime;
    // Start is called before the first frame update
    void Start()
    {
        starttime = Time.time;
        
    }

    // Update is called once per frame
    void Update()
    {
        endtime = Time.time;
        
        
        if (2 - (endtime-starttime) > 0)
        {
            this.gameObject.GetComponent<TrailRenderer>().widthMultiplier = 5 * (2 - Time.time);
        }
        
    }
}
