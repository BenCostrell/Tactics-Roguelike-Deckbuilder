using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DottedLine : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 start;
    private const int pointDensity = 10;
    private const float curveAngle = 20;

    // Start is called before the first frame update
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }


    void Update()
    {
        Vector3 target = transform.position;
        Vector3[] points = new Vector3[Mathf.RoundToInt(pointDensity * Vector3.Distance(start, target))];
        for (int i = 0; i < points.Length; i++)
        {
            //float x = 
        }
        lr.SetPositions(points);

    }
    
}
