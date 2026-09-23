using UnityEngine;

/*
 * QuadraticBezierCurve owns the three control-point Transforms in the Demo
 * scene. This is where their world positions connect to quadratic math,
 * Scene-view gizmos, and the Play Mode line.
 */

public class QuadraticBezierCurve : MonoBehaviour
{
    [Header("Bezier Points")]
    public Transform p0;
    public Transform p1;
    public Transform p2;

    public int numSamples = 10;

    void Start()
    {
        // TODO Slice 1.7: draw this curve in its LineRenderer with numSamples points.
        // Space them evenly in t and include both endpoints.
        // Check: restart Play Mode. The line matches the Scene-view gizmos.
        // Next: Slice 2.1 in FollowCurve.cs. </> end of Slice 1

        // TODO Slice 7.3 (upgrade 1.7): draw the line with power-basis evaluation.
        // Prepare the coefficients once, outside the sample loop.
        // Check: restart Play Mode. The line still matches the gizmos,
        // and every sample uses power evaluation.
        // Next: Slice 7.4 in Bezier/QuadraticBezierMath.cs.
    }

    void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null) return;
        // TODO Slice 1.2: draw this curve's gizmos with CurveGizmos.Draw.
        // Its curve step (1.6) samples this curve.
        // Check: turn on Scene-view Gizmos. Three markers sit on the points.
        // Move a point outside Play Mode; its marker follows.
        // Next: Slice 1.3 in CurveGizmos.cs.

        CurveGizmos.Draw(10, SamplePoint, p0, p1, p2);
    }

    public Vector3 SamplePoint(float t)
    {
        // TODO Slice 1.5: sample your De Casteljau quadratic at the control points'
        // current world positions.
        // Check: SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(False,False) passes.
        // Next: Slice 1.6 in CurveGizmos.cs.

        Vector3 sample = QuadraticBezierMath.SamplePointDeCasteljau(p0.position, p1.position, p2.position, t);

        // TODO Slice 6.2 (upgrade 1.5): switch to your Bernstein evaluator.
        // Check: the curve and follower look the same. This method calls Bernstein; keep it.
        // Next: Slice 6.3 in Bezier/QuadraticBezierMath.cs.
        return sample;
    }

    public Vector3 SampleTangent(float t)
    {
        // TODO Slice 2.3: sample your De Casteljau derivative at the control points'
        // current world positions. Do not normalize it.
        // Check: SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(False,True) passes.
        // Next: Slice 2.4 in FollowCurve.cs.

        // TODO Slice 6.4 (upgrade 2.3): switch to your Bernstein derivative.
        // Check: the follower faces the same way. This method calls Bernstein; keep it.
        // Next: Slice 7.1 in Bezier/QuadraticBezierMath.cs. </> end of Slice 6
        return Vector3.zero;
    }
}
