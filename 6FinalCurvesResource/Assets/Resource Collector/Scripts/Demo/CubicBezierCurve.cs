using UnityEngine;

/*
 * CubicBezierCurve owns the four control-point Transforms in the Demo scene.
 * This is where their world positions connect to CubicBezierMath, Scene-view
 * gizmos, and the Play Mode line.
 */

public class CubicBezierCurve : MonoBehaviour
{
    [Header("Bezier Points")]
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;

    public int numSamples = 10;

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numSamples;

        //Vector3 lastSample = p0.position;
        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i / (numSamples - 1);
            lineRenderer.SetPosition(i, SamplePoint(t));
        }
    }

    void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null || p3 == null) return;
        CurveGizmos.Draw(10, SamplePoint, p0, p1, p2, p3);
    }

    public Vector3 SamplePoint(float t)
    {
        return CubicBezierMath.SamplePoint(p0.position, p1.position, p2.position, p3.position, t);
    }

    public Vector3 SampleTangent(float t)
    {

        return CubicBezierMath.SampleTangent(p0.position, p1.position, p2.position, p3.position, t);
    }
}
