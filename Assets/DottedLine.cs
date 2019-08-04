using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DottedLine : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 start;
    private Vector3 target;
    private const int pointDensity = 10;
    private const float curveAngle = 20;
    private const float lineZ = -2;
    public Transform arrow;

    // Start is called before the first frame update
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }


    void Update()
    {
        Vector3 diff = target - start;
        int numPoints = Mathf.CeilToInt(pointDensity * Vector3.Distance(start, target));
        Vector3[] points = new Vector3[numPoints];
        lr.positionCount = numPoints;
        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / numPoints;
            float x = start.x + (t * diff.x);
            float y = start.y + (t * diff.y);
            points[i] = new Vector3(x, y, lineZ);
        }
        //Vector3[] points = new Vector3[] { start, target };
        lr.SetPositions(points);
    }

    public void SetTarget(Vector3 start_, Vector3 target_)
    {
        start = new Vector3(start_.x, start_.y, lineZ);
        target = new Vector3(target_.x, target_.y, lineZ);
        arrow.position = target_;
        Vector3 diff = target - start;
        arrow.localRotation = Quaternion.Euler(0, 0, 
            Mathf.Rad2Deg * (-Mathf.PI/2 + Mathf.Atan2(diff.y, diff.x)));
    }
    
}
