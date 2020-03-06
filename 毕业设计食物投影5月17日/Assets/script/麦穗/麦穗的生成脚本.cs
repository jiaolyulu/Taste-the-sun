using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 麦穗的生成脚本 : MonoBehaviour
{
    public 麦穗的生长控制脚本 Grow;
    public 麦穗掉落的生长控制脚本 FallGrow;
    public Transform anchor;
    public float rotation;
    public int addpre;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Grow.add>addpre)
        {
            rotation = Random.Range(0, 72);
            rotation = rotation * 5;
            int i = Random.Range(0, 5);
            if (i <=2)
            {
                GameObject Maisuiprefab = Resources.Load("麦穗") as GameObject;
                GameObject MaiSui;
                Quaternion rotate = Quaternion.Euler(new Vector3(0, 0, rotation));
                MaiSui = Instantiate(Maisuiprefab, anchor.position, rotate) as GameObject;
                float m = Random.Range(1f, 1.5f);
                MaiSui.transform.GetChild(0).gameObject.transform.localScale = new Vector3(m, m, m);
                
                MaiSui.transform.parent = this.transform;
                MaiSui.transform.localScale = new Vector3(1, 1, 1);
                MaiSui.transform.SetSiblingIndex(0);
                addpre = Grow.add;

            }
            if (i == 3)
            {
                GameObject Maisuiprefab = Resources.Load("麦穗1") as GameObject;
                GameObject MaiSui1;
                Quaternion rotate = Quaternion.Euler(new Vector3(0, 0, rotation));
                MaiSui1 = Instantiate(Maisuiprefab, anchor.position, rotate) as GameObject;
                float m = Random.Range(1.2f, 1.5f);
                MaiSui1.transform.GetChild(0).gameObject.transform.localScale = new Vector3(m, m, m);
                MaiSui1.transform.parent = this.transform;
                MaiSui1.transform.localScale = new Vector3(1, 1, 1);
                MaiSui1.transform.SetSiblingIndex(0);
           

                addpre = Grow.add;
            }

              
            
        }

    }
}
