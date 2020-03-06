using UnityEngine;
using System.Collections.Generic;

public class PathBezier : MonoBehaviour 
{
    [SerializeField]
    PathPoint[] _controlPoints = new PathPoint[0];
    public Color lineColour = Color.white;
    public PathPoint[] controlPoints
    {
        get { return _controlPoints; }
    }
    public int numberOfControlPoints
    {
        get
        {
            if (controlPoints != null)
                return controlPoints.Length;
            else
                return 0;
        }
    }
    [SerializeField]
    float[] _storedArcLengths = null;
    [SerializeField]
    int storedArcLengthArraySize = 750;
    [SerializeField]
    float[] _storedArcLengthsFull = null;
    [SerializeField]
    float _storedTotalArcLength = 0;

    Quaternion lastRotation = Quaternion.identity;
    int overRotate = 0;
    public float storedTotalArcLength
    {
        get { return _storedTotalArcLength; }
    }
    public bool isLoop = false;
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = lineColour;
    //    if (numberOfUseablePoints < 1)
    //        return;

    //    for (float t = 0.0f; t <= 1.0f; t += 0.015f)
    //    {
    //        Gizmos.DrawLine(GetPathPosition(t), GetPathPosition(t + 0.013f));
    //    }
    //}
    void OnEnable()
    {
        RecalculateStoredValues();
    }
    void Start()
    {
        RecalculateStoredValues();
    }
    public int numberOfUseablePoints
    {
        get
        {
            if (!isLoop)
            {
                return _controlPoints.Length - 1;
            }
            else
            {
                return _controlPoints.Length;
            }
        }
    }
    public void AddPoint()
    {
        AddPoint(_controlPoints.Length);
    }
    public void AddPoint(int index)
    {
        GameObject newPoint = new GameObject();
        newPoint.transform.parent = transform;
        if(numberOfControlPoints == 0)
        {
            newPoint.transform.localPosition = Vector3.zero;
        }
        else if (numberOfControlPoints == 1)
        {
            newPoint.transform.localPosition = _controlPoints[0].transform.rotation * Vector3.forward * 5f;
        }
        else
        {
            if (index < numberOfControlPoints)
            {
                PathPoint pointA = _controlPoints[Mathf.Clamp(index - 1, 0, numberOfUseablePoints)];
                PathPoint pointB = _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)];
                Vector3 p0 = pointA.transform.position;
                Vector3 p1 = pointA.worldControlPoint;
                Vector3 p2 = pointB.worldControlPoint;
                Vector3 p3 = pointB.transform.position;
                newPoint.transform.position = CalculateBezierPoint(0.5f, p0, p1, p2, p3);

                Quaternion p = _controlPoints[Mathf.Clamp(index - 2, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion q = _controlPoints[Mathf.Clamp(index - 1, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion a = _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)].transform.rotation;
                Quaternion b = _controlPoints[Mathf.Clamp(index + 1, 0, numberOfUseablePoints)].transform.rotation;
                newPoint.transform.rotation = CalculateCubicRotation(p, a, b, q, 0.5f);
            }
            else
            {
                Vector3 endDirection = (GetPathPosition(1f) - GetPathPosition(0.95f)).normalized;
                newPoint.transform.position = _controlPoints[index - 1].transform.position + endDirection * 5f;
                newPoint.transform.rotation = _controlPoints[index - 1].transform.rotation;
            }
        }
        PathPoint cpScript = newPoint.AddComponent<PathPoint>();
        cpScript.name = "control point " + index;
        cpScript.bezier = this;
        List<PathPoint> rebuildList = new List<PathPoint>(_controlPoints);
        rebuildList.Insert(index, cpScript);
        _controlPoints = rebuildList.ToArray();
        RecalculateStoredValues(); 
    }
    public void DeletePoint(PathPoint deletePoint, bool destroy)
    {
        List<PathPoint> rebuildList = new List<PathPoint>(_controlPoints);
        rebuildList.Remove(deletePoint);
        _controlPoints = rebuildList.ToArray();
        if (destroy)
            DestroyImmediate(deletePoint.gameObject);
        RecalculateStoredValues();
    }
    public void DeletePoint(int index, bool destroy)
    {
        List<PathPoint> rebuildList = new List<PathPoint>(_controlPoints);
        PathPoint deletePoint = rebuildList[index];
        rebuildList.RemoveAt(index);
        _controlPoints = new PathPoint[rebuildList.Count];
        _controlPoints = rebuildList.ToArray();
        if (destroy)
            DestroyImmediate(deletePoint.gameObject);
        RecalculateStoredValues();
    }
    public void RecalculateStoredValues()
    {
        if(_controlPoints.Length < 2)
        {
            return;
        }
        float curveT;
        if (numberOfUseablePoints < 1)
        {
            curveT = 1f;
        }
        else
        {
            curveT = 1f/(float)numberOfUseablePoints;
        }
        _storedArcLengths = new float[numberOfUseablePoints];
        for (int i = 0; i < numberOfUseablePoints; ++i)
        {
            _storedArcLengths[i] = 0;
        }
        float alTime = 1f/(storedArcLengthArraySize);
        float calculatedTotalArcLength = 0;
        _storedArcLengthsFull = new float[storedArcLengthArraySize];
        _storedArcLengthsFull[0] = 0.0f;
        for (int i = 0; i < storedArcLengthArraySize - 1; i++)
        {
            float altA = alTime * (i + 1);
            float altB = alTime * (i + 1) + alTime;
            Vector3 pA = GetPathPosition(altA);
            Vector3 pB = GetPathPosition(altB);
            float arcLength = Vector3.Distance(pA, pB);
            int arcpoint = Mathf.FloorToInt(altA / curveT);
            calculatedTotalArcLength += arcLength;
            _storedArcLengths[arcpoint] += arcLength;
            _storedArcLengthsFull[i + 1] = calculatedTotalArcLength;
        }
        _storedTotalArcLength = calculatedTotalArcLength;
    }
    public Vector3 GetPathPosition(float t)
    {
        float curveT = 1f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        PathPoint pointA, pointB;
        pointA = GetPoint(point);
        pointB = GetPoint(point + 1);
        return CalculateBezierPoint(ct, pointA.transform.position, pointA.worldControlPoint,
            pointB.reverseWorldControlPoint, pointB.transform.position);
    }
    public Quaternion GetPathRotation(float t)
    {
        float curveT = 1f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        Quaternion p, q, a, b;
        p = GetPoint(point).transform.rotation;
        q = GetPoint(point + 1).transform.rotation;
        a = GetPoint(point - 1).transform.rotation;
        b = GetPoint(point + 2).transform.rotation;

        Quaternion ret = CalculateCubicRotation(p, a, b, q, ct);
        if(lastRotation != Quaternion.identity)
        {
            if (Quaternion.Angle(ret, lastRotation) > 90 && overRotate > 5)
            {
                ret = lastRotation;
                overRotate++;
            }
            else
            {
                overRotate = 0;
            }
        }
        lastRotation = ret;
        return ret;
    }
    public float GetPathFieldOfView(float t)
    {
        float curveT = 1f / (float)numberOfUseablePoints;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfUseablePoints);

        float fovA = GetPoint(point).fieldOfView;
        float fovB = GetPoint(point + 1).fieldOfView;
        return Mathf.Lerp(fovA, fovB, CalculateHermite(ct));
    }
    float CalculateHermite(float val)
    {
        return val * val * (3.0f - 2.0f * val);
    }
    PathPoint GetPoint(int index)
    {
        if(_controlPoints.Length == 0)
        {
            return null;
        }
        if (isLoop)
        {
            if (index >= numberOfControlPoints)
                index = index - numberOfControlPoints;
            if (index < 0)
                index = index + numberOfControlPoints;
            return _controlPoints[index];
        }
        else
        {
            return _controlPoints[Mathf.Clamp(index, 0, numberOfUseablePoints)];
        }
    }
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        float u = 1.0f - t;
        float u2 = u * u;
        float u3 = u2 * u;
        Vector3 p = u3 * p0 + 3 * u2 * t * p1 + 3 * u * t2 * p2 + t3 * p3;
        return p;
    }
    Quaternion CalculateCubicRotation(Quaternion p, Quaternion a, Quaternion b, Quaternion q, float t)
    {
        Quaternion a1 = SquadTangent(a, p, q);
        Quaternion b1 = SquadTangent(p, q, b);
        float slerpT = 2.0f * t * (1.0f - t);
        Quaternion sl = Slerp(Slerp(p, q, t), Slerp(a1, b1, t), slerpT);
        return sl;
    }
    Quaternion Slerp(Quaternion p, Quaternion q, float t)
    {
        Quaternion ret;
        float cos = Quaternion.Dot(p, q);

        float omega, somega, invSin, fCoeff0, fCoeff1;
        if ((1.0f + cos) > 0.00001f)
        {
            if ((1.0f - cos) > 0.00001f)
            {
                omega = Mathf.Acos(cos);
                somega = Mathf.Sin(omega);
                invSin = (Mathf.Sign(somega) * 1.0f) / somega;
                fCoeff0 = Mathf.Sin((1.0f - t) * omega) * invSin;
                fCoeff1 = Mathf.Sin(t * omega) * invSin;
            }
            else
            {
                fCoeff0 = 1.0f - t;
                fCoeff1 = t;
            }
            ret.x = fCoeff0 * p.x + fCoeff1 * q.x;
            ret.y = fCoeff0 * p.y + fCoeff1 * q.y;
            ret.z = fCoeff0 * p.z + fCoeff1 * q.z;
            ret.w = fCoeff0 * p.w + fCoeff1 * q.w;
        }
        else
        {
            fCoeff0 = Mathf.Sin((1.0f - t) * Mathf.PI * 0.5f);
            fCoeff1 = Mathf.Sin(t * Mathf.PI * 0.5f);

            ret.x = fCoeff0 * p.x - fCoeff1 * p.y;
            ret.y = fCoeff0 * p.y + fCoeff1 * p.x;
            ret.z = fCoeff0 * p.z - fCoeff1 * p.w;
            ret.w = p.z;
        }
        return ret;
    }
    Quaternion SquadTangent(Quaternion before, Quaternion center, Quaternion after)
    {
        Quaternion l1 = lnDif(center, before);
        Quaternion l2 = lnDif(center, after);
        Quaternion e = Quaternion.identity;
        for (int i = 0; i < 4; ++i)
        {
            e[i] = -0.25f * (l1[i] + l2[i]);
        }
        return center * (exp(e));
    }
    Quaternion lnDif(Quaternion a, Quaternion b)
    {
        Quaternion dif = Quaternion.Inverse(a) * b;
        Normalize(dif);
        return log(dif);
    }
    Quaternion Normalize(Quaternion q)
    {
        float norm = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if (norm > 0.0f)
        {
            q.x /= norm;
            q.y /= norm;
            q.z /= norm;
            q.w /= norm;
        }
        else
        {
            q.x = (float)0.0f;
            q.y = (float)0.0f;
            q.z = (float)0.0f;
            q.w = (float)1.0f;
        }
        return q;
    }
    Quaternion exp(Quaternion q)
    {
        float theta = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

        if (theta < 1E-6)
        {
            return new Quaternion(q[0], q[1], q[2], Mathf.Cos(theta));
        }
        else
        {
            float coef = Mathf.Sin(theta) / theta;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, Mathf.Cos(theta));
        }
    }
    Quaternion log(Quaternion q)
    {
        float len = Mathf.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2]);

        if (len < 1E-6)
        {
            return new Quaternion(q[0], q[1], q[2], 0.0f);
        }
        else
        {
            float coef = Mathf.Acos(q[3]) / len;
            return new Quaternion(q[0] * coef, q[1] * coef, q[2] * coef, 0.0f);
        }
    }
}
