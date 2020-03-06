
using UnityEngine;

using System.Collections;



public class curvemovement : MonoBehaviour
{



    public GameObject t1; //开始位置

    public GameObject t2; //结束位置

    // Update is called once per frame

    void Update()
    {



        //两者中心点

        Vector3 center = (t1.transform.position + t2.transform.position) * 0.5f;
        Vector3 direction = t1.transform.position - t2.transform.position;
        Vector3 vec3 = -direction;
        float m = t2.transform.position.x * direction.y - t2.transform.position.y * direction.x;
        float k = Random.Range(0.1f, 0.2f);
        if (m < 0)
        {
            center += new Vector3(k * direction.y, k * direction.x, 0);
        }
        else
        {
            center -= new Vector3(k * direction.y, k * direction.x, 0);
        }
        



        Vector3 start = t1.transform.position - center;

        Vector3 end = t2.transform.position - center;



        //弧形插值

        transform.position = Vector3.Slerp(start, end, 0.1f*Time.time);

        transform.position += center;



    }

}