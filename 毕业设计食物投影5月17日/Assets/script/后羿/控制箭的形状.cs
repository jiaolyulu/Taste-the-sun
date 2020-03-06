using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 控制箭的形状 : MonoBehaviour
{
    public GameObject jianshang;
    public GameObject jianxia;
    public GameObject zuoshou;
    public GameObject youshou;
    float ycha, xcha;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = zuoshou.transform.position;
        ycha = youshou.transform.position.y - zuoshou.transform.position.y;
        xcha = youshou.transform.position.x - zuoshou.transform.position.x;
        if ((ycha>0)&(xcha>0))
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180 / Mathf.PI * Mathf.Atan(ycha / xcha));
        }
        if ((ycha < 0) & (xcha > 0))
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180 / Mathf.PI * Mathf.Atan(ycha / xcha));
        }
        if ((ycha < 0) & (xcha < 0))
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 180+(180 / Mathf.PI * Mathf.Atan(ycha / xcha)));
        }
        if ((ycha > 0) & (xcha < 0))
        {
            this.transform.localEulerAngles = new Vector3(0, 0, 90-(180 / Mathf.PI * Mathf.Atan(ycha / xcha)));
        }
    }
}
