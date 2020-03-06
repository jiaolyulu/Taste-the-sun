using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 贝塞尔曲线 : MonoBehaviour
{
    public Transform start;
    public Transform end;
    Vector3 p0,p1,p2,p3;
    Vector3 vec1, vec2,vec3;
    
    float t;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        p0 = start.GetChild(0).transform.position;
        p3 = end.position;
        time = Random.Range(1.5f,2.5f);
        
        vec1 = end.position;
        vec2 = start.position - end.position;
        vec3 = -vec2;
        float m = vec1.x * vec2.y - vec1.y * vec2. x;
        float k = Random.Range(0.25f, 0.50f);
        if (m <= 0)
        {
            float a = vec3.x;
            float b = vec3.y;
            Vector3 vec4 = new Vector3 (0.5f*b,-0.5f*a);
            p1= 0.7f * end.position+0.3f*start.position + k*vec4;
            p2= 0.3f * end.position + 0.7f * start.position + k * vec4;

        }
        if (m > 0)
        {
            float a = vec3.x;
            float b = vec3.y;
            Vector3 vec4 = new Vector3(-0.5f*b, 0.5f * a);
            p1 = 0.3f * end.position + 0.7f * start.position + k * vec4;
            p2 = 0.7f * end.position + 0.3f * start.position + k * vec4;

        }
       
        t = 0;

    }

    // Update is called once per frame
    void Update()
    {
       
            
            t += Time.deltaTime;
            float f = t / time;
            if (f <= 1)
            {
                this.transform.position = (1 - f) * (1 - f) * (1 - f) * p0 + 3 * (1 - f) * (1 - f) * f * p1 + 3 * (1 - f) * f * f * p2 + f * f * f * p3;
            }
            if (t > (time + 1))
            {
                Destroy(this.gameObject);
            }
            //如果被按，update执行判断，然后生成物体。生成物的参数里要分配起始位置和结束位置。
            //生成的物体自己的update里出现这个贝塞尔曲线。
       





    }
}
