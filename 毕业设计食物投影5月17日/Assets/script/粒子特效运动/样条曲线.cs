using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 样条曲线 : MonoBehaviour
{
    public float[] x = { 0,20,40 };
    public float[] y = {0,40,40 };
    float[] a;
    float[] b;
    float[] c;
    float[] d;
    float[] h;
    float[] xExt;
    float[] yExt;
    int i;
    // Start is called before the first frame update
    static float[] TDMA(float[] ta, float[] tb, float[] tc, float[] tx)
    {
        int n = tx.Length;
        tc[0] = tc[0] / tb[0];
        tx[0] = tx[0] / tb[0];

        for (int i = 1; i < n; i++)
        {
            float m = 1 / (tb[i] - ta[i] * tc[i - 1]);
            tc[i] = tc[i] * m;
            tx[i] = (tx[i] - ta[i] * tx[i - 1]) * m;
        }
        for (int i = n - 2; i > 0; i--)
        {
            tx[i] = tx[i] - tc[i] * tx[i + 1];
        }
        return tx;
    }


    private float[] Xinterp(float[] xin, float[] hin)
    {
        float[] xExt = new float[(xin.Length - 1) * 3 + 1];
        int i = 0;
        for (; i < (xin.Length - 1); i++)
        {
            xExt[i * 3] = xin[i];
            xExt[i * 3 + 1] = xin[i] + hin[i] / 3;
            xExt[i * 3 + 2] = xin[i] + hin[i] / 3 * 2;
        }
        xExt[i * 3] = xin[i];
        return xExt;
    }

    private float[] Yinterp(float[] xex)
    {
        float[] yout = new float[xex.Length];
        for (int i = 0; i < xex.Length; i++)
        {
            int seg = (int)i / 3;
            float h = xex[i] - xex[seg * 3];
            yout[i] = a[seg] + b[seg] * h + c[seg] * h * h + d[seg] * h * h * h;
        }
        yout[xex.Length - 1] = y[y.Length - 1];
        return yout;
    }

    private void Cinterp(float[] x, float[] y)
    {
        int n = x.Length;
        float[] m = new float[n];
        h = new float[n - 1];
        a = new float[n];
        b = new float[n];
        c = new float[n];
        d = new float[n];
        for (int i = 0; i < n - 1; i++)
        {
            h[i] = x[i + 1] - x[i];
        }
        a[0] = 0;
        b[0] = 1;
        c[0] = 0;
        d[0] = 0;
        a[n - 1] = 0;
        b[n - 1] = 1;
        c[n - 1] = 0;
        d[n - 1] = 0;
        for (int i = 1; i < n - 1; i++)
        {
            a[i] = h[i - 1];
            b[i] = 2 * (h[i - 1] + h[i]);
            c[i] = h[i];
            d[i] = 6 * ((y[i + 1] - y[i]) / h[i] - (y[i] - y[i - 1]) / h[i - 1]);
        }
        m = TDMA(a, b, c, d);
        for (int i = 0; i < n - 1; i++)
        {
            a[i] = y[i];
            b[i] = (y[i + 1] - y[i]) / h[i] - h[i] * m[i] / 2 - h[i] * (m[i + 1] - m[i]) / 6;
            c[i] = m[i] / 2;
            d[i] = (m[i + 1] - m[i]) / (6 * h[i]);
        }
    }

    void Start()
    {
        Cinterp(x, y);
        for (int i = 0; i < (x.Length - 1); i++)
        {
            //            Debug.Log(a[i]+","+b[i]+","+c[i]+","+d[i]);
        }
        xExt = Xinterp(x, h);
        yExt = Yinterp(xExt);

        i = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (i < xExt.Length )
        { this.transform.position = new Vector3(xExt[i], yExt[i]); i++; }

        

    }
}
