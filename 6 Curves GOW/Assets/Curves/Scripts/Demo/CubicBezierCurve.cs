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

    void Start()
    {
        // TODO Slice 3.4: draw this cubic in its LineRenderer with numSamples points.
        // Space them evenly in t and include both endpoints.
        // Check: restart Play Mode. The line matches the gizmos and reaches p0 and p3.
        // Next: Slice 3.5 in Bezier/CubicBezierMath.cs.
    }

    void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null || p3 == null) return;
        // TODO Slice 3.3: draw this cubic's gizmos with your CurveGizmos helper.
        // Check: four markers, an open three-segment polygon, and a curve that reaches
        // both ends. Move a point outside Play Mode; everything follows.
        // Next: Slice 3.4 in Start.
    }

    public Vector3 SamplePoint(float t)
    {
        // TODO Slice 3.2: sample your cubic evaluator at the control points'
        // current world positions.
        // Check: SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(True,False) passes.
        // Next: Slice 3.3 in OnDrawGizmos.
        return Vector3.zero;
    }

    public Vector3 SampleTangent(float t)
    {
        // TODO Slice 3.6: sample your cubic derivative at the control points'
        // current world positions. Do not normalize it.
        // Check: SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(True,True) passes.
        // All four cubic tests pass.
        // Next: open Main Game, Slice 4.1 in Game/ThrownAxe.cs. </> end of Slice 3
        return Vector3.zero;
    }
}
