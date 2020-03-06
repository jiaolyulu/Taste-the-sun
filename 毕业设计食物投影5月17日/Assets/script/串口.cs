using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class 串口 : MonoBehaviour
{
    存在信息 sun_exist;
    public int recent_press;
    private SerialPort sp;
    private Thread receiveThread;  //用于接收消息的线程
    private Thread sendThread;     //用于发送消息的线程
    string[] stack;
    public string[] mode ;
    public float[] start_time;
    public float[] end_time;
    public float 时间;
    public int now_pressing;
    GameObject zhiwuzu;
    public  Animator dabeijing;
    // Start is called before the first frame update
    void Start()
    {
        dabeijing = GameObject.Find("Canvas/大背景").GetComponent<Animator>();
        
        sun_exist = GameObject.Find("太阳的存在状态").GetComponent<存在信息>();
        stack = new string[8];
        start_time = new float[8];
        end_time = new float[8];
        mode = new string[8] { "空闲", "空闲", "空闲", "空闲", "空闲", "空闲", "空闲", "空闲" };

        sp = new SerialPort("COM3", 9600);  //这里的"COM4"是我的设备，可以在设备管理器查看。
                                            //9600是默认的波特率，需要和Arduino对应，其它的构造参数可以不用配置。
        sp.ReadTimeout = 500;
        sp.Open();
        receiveThread = new Thread(ReceiveThread);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        zhiwuzu = GameObject.Find("Canvas/麦穗中点");
        //sendThread = new Thread(SendThread);
        //sendThread.IsBackground = true;
        //sendThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        for (int k = 0; k <= 7; k++)
        {
            if ((mode[k]=="按下开始了")||(mode[k]=="按下"))
            {
                int shifou = UnityEngine.Random.Range(0, 8);
                if (shifou == 4)
                {
                    int possibility = UnityEngine.Random.Range(0, zhiwuzu.transform.childCount - 1);
                    GameObject daosui = zhiwuzu.transform.GetChild(possibility).gameObject;
                    GameObject liziprefab = Resources.Load("尾端粒子") as GameObject;
                    GameObject lizi;
                    Quaternion rotate = Quaternion.Euler(new Vector3(0, 0, 0));
                    lizi = Instantiate(liziprefab, daosui.gameObject.transform.GetChild(0).transform.position, rotate) as GameObject;
                    lizi.transform.parent = GameObject.Find("粒子特效集合").transform;
                    lizi.GetComponent<曲线运动>().target = GameObject.Find("太阳位置/太阳" + ((k + 1).ToString()));
                }
                
               
            }
        }
        时间 = Time.time;
        if (recent_press == 10)
        {
            dabeijing.SetBool("rotation", false);

        }
        else
        {
            dabeijing.SetBool("rotation", true);
        }
        if (sun_exist.N == 0)
        {
            if (now_pressing != 0)
            {
                for (int j = 1; j <= 8; j++)
                {
                   
                        Animator bluetree_ani = GameObject.Find("Canvas/蓝色背景遮罩/太阳" + j.ToString()).transform.GetChild(0).GetComponent<Animator>();
                        GameObject liziforcefield = GameObject.Find("太阳/太阳" + j.ToString()).transform.GetChild(1).gameObject;
                        liziforcefield.GetComponent<ParticleSystemForceField>().endRange = 56;
                        bluetree_ani.SetBool("xiaoshi", false);
                        bluetree_ani.SetBool("chuxian", true);
                        sun_exist.sun[j-1] = true;
                    
                   
                }
            }
        }
    }

    //private void SendThread()
    //{
    //    while (true)
    //    {
    //        Thread.Sleep(20);
    //        this.sp.DiscardInBuffer();
    //        if (i > 255)
    //            i = 0;
    //        sp.WriteLine(i++.ToString());
    //        Debug.Log(i++.ToString());
    //    }
    //}
    void ReceiveThread()
    {
        while (true)
        {
            if (this.sp != null && this.sp.IsOpen)
            {
                try
                {
                    String strRec = sp.ReadLine();            //SerialPort读取数据有多种方法，我这里根据需要使用了ReadLine()
                    //Debug.Log("Receive From Serial: " + strRec);
                    for (int i = 0; i<= 7; i++)
                    {
                        char m = strRec[i];
                        string m_s = m.ToString();
                        stack[i] = stack[i] + m_s;
                        if (stack[i].Length >= 7)
                        {
                            stack[i] = stack[i].Substring(stack[i].Length-7,7);
                        }
                        if (stack[i] == "0001111")
                        {
                            mode[i] = "按下开始了";
                            start_time [i]= 时间;
                        }
                        if (stack[i] == "1111111")
                        {
                            mode[i] = "按下";
                        }
                        if (stack[i] == "1111000")
                        {
                            mode[i]= "抬起";
                            Debug.Log("抬起");
                            end_time[i] = 时间;
                            
                            if (end_time[i] - start_time[i] >= 1)
                            
                            {
                                sun_exist.sun[i] = false;
                            }
                        }
                        if ( stack[i]=="0000000")
                        {
                            mode[i] = "空闲";
                        }
                        //int m1 = int.Parse(m_s);
                        //if (m1 == 0)
                        //{

                        //}
                        //if (m1 == 1)
                        //{

                        //}


                    }
                    float starttime = 0;
                    now_pressing = 0;
                    for (int j= 0; j <= 7; j++)
                    {
                        if ((mode[j] == "按下")|| (mode[j] == "按下开始了"))
                        {
                            now_pressing++;
                            if (start_time[j] > starttime)
                            {
                                recent_press = j;
                                starttime = start_time[j];

                            }
                        }
                    }
                    if (starttime==0)
                    { recent_press = 10; }
                   
                    
                }
                catch
                {
                    //continue;
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        sp.Close();//关闭串口
    }

   

}
