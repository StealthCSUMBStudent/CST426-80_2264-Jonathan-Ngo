using UnityEngine;

/*
 * CubicBezierMath holds De Casteljau point and tangent sampling for four
 * control points. CubicBezierCurve supplies their current scene positions.
 */

public static class CubicBezierMath
{
    public static Vector3 SamplePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // TODO Slice 3.1: evaluate the cubic at t with De Casteljau. Slice 3 is on your own.
        // Check: DeCasteljauCubic_SamplesPointFromEquivalentCubicFormula passes.
        // Next: Slice 3.2 in Demo/CubicBezierCurve.cs.
        return Vector3.zero;
    }

    public static Vector3 SampleTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // TODO Slice 3.5: return the cubic derivative at t from your De Casteljau
        // construction. Do not normalize it.
        // Stuck? Show ChatGPT your construction and ask: "Differentiate this with respect
        // to t using the product rule. Explain each step." Then derive it yourself.
        // Check: DeCasteljauCubic_SamplesTangentFromFinalInterpolationSegment passes.
        // Next: Slice 3.6 in Demo/CubicBezierCurve.cs.
        return Vector3.zero;
    }
}
