using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PathPoint : MonoBehaviour 
{
    public PathBezier bezier;
    public Vector3 controlPoint = Vector3.zero;
    public float fieldOfView;//摄像机视野.
    public Vector3 storedPosition = Vector3.zero;
    public Vector3 worldControlPoint
    {
        get
        {
            Vector3 returnPoint = bezier.transform.rotation * controlPoint;
            returnPoint += transform.position;
            return returnPoint;
        }
        set
        {
            Vector3 newValue = value - transform.position;
            newValue = Quaternion.Inverse(bezier.transform.rotation) * newValue;
            controlPoint = newValue;
        }
    }
    public Vector3 reverseWorldControlPoint
    {
        get
        {
            Vector3 returnPoint = bezier.transform.rotation * -controlPoint;
            returnPoint += transform.position;
            return returnPoint;
        }
        set
        {
            Vector3 newValue = -value - transform.position;
            newValue = Quaternion.Inverse(bezier.transform.rotation) * newValue;
            controlPoint = newValue;
        }
    }
    void OnDestroy()
    {
        if (bezier != null)
        {
            bezier.DeletePoint(this, false);
        }
    }
}
