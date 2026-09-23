using UnityEngine;

/*
 * FollowCurve tracks time for a trip along a QuadraticBezierCurve. Its Update
 * loop is where movement and facing belong; triggerReset starts the trip again.
 */

public class FollowCurve : MonoBehaviour
{
    public QuadraticBezierCurve curve;
    public float duration = 3f;
    public bool triggerReset;

    float _elapsed;

    void Update()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / duration);
        // TODO Slice 2.1: move this object to the curve at t.
        // Check: the follower travels along the line. Tick triggerReset to repeat.
        // Next: Slice 2.2 in Bezier/QuadraticBezierMath.cs.

        // TODO Slice 2.4: face this object along the curve, using your 2.3 tangent.
        // Check: the follower faces along the curve, with no zero-direction warning.
        // Next: Slice 3.1 in Bezier/CubicBezierMath.cs. </> end of Slice 2

        if (triggerReset)
        {
            triggerReset = false;
            _elapsed = 0;
        }
    }
}
