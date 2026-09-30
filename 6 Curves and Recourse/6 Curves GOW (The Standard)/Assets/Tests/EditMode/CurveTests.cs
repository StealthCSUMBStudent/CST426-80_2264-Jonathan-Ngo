using NUnit.Framework;
using UnityEngine;

public class CurveTests
{
    const float Tolerance = 0.0001f;

    GameObject _curveObject;

    [TearDown]
    public void TearDown()
    {
        if (_curveObject != null) Object.DestroyImmediate(_curveObject);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(bool cubic, bool tangent)
    {
        _curveObject = new GameObject("Curve");
        _curveObject.SetActive(false);
        _curveObject.transform.position = new Vector3(10f, -4f, 2f);
        _curveObject.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        _curveObject.transform.localScale = new Vector3(2f, 3f, 4f);
        Transform p0 = CreatePoint("p0", Vector3.zero);
        Transform p1 = CreatePoint("p1", cubic ? new Vector3(1f, 3f, 0f) : new Vector3(2f, 3f, 0f));
        Transform p2 = CreatePoint("p2", cubic ? new Vector3(3f, 3f, 0f) : new Vector3(4f, 0f, 0f));
        QuadraticBezierCurve quadratic = null;
        CubicBezierCurve cubicCurve = null;
        if (cubic)
        {
            cubicCurve = _curveObject.AddComponent<CubicBezierCurve>();
            cubicCurve.p0 = p0;
            cubicCurve.p1 = p1;
            cubicCurve.p2 = p2;
            cubicCurve.p3 = CreatePoint("p3", new Vector3(4f, 0f, 0f));
        }
        else
        {
            quadratic = _curveObject.AddComponent<QuadraticBezierCurve>();
            quadratic.p0 = p0;
            quadratic.p1 = p1;
            quadratic.p2 = p2;
        }

        Vector3 Sample(float t) => tangent
            ? (cubic ? cubicCurve.SampleTangent(t) : quadratic.SampleTangent(t))
            : (cubic ? cubicCurve.SamplePoint(t) : quadratic.SamplePoint(t));

        Vector3 expected = tangent
            ? (cubic ? new Vector3(4.125f, 4.5f, 0f) : new Vector3(4f, 3f, 0f))
            : (cubic ? new Vector3(0.90625f, 1.6875f, 0f) : new Vector3(1f, 1.125f, 0f));
        AssertVector3(expected, Sample(0.25f));

        // Change one control point; each method must refresh independently.
        p1.position += new Vector3(0f, 0f, 2f);
        expected.z = tangent ? (cubic ? 1.125f : 2f) : (cubic ? 0.84375f : 0.75f);
        AssertVector3(expected, Sample(0.25f));
    }

    Transform CreatePoint(string name, Vector3 worldPosition)
    {
        Transform point = new GameObject(name).transform;
        point.SetParent(_curveObject.transform);
        point.position = worldPosition;
        return point;
    }

    [Test]
    public void DeCasteljauQuadratic_SamplesPointFromEquivalentQuadraticFormula()
    {
        Vector3 result = QuadraticBezierMath.SamplePointDeCasteljau(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(1f, 1.125f, 0f), result);
    }

    [Test]
    public void DeCasteljauQuadratic_SamplesTangentFromFinalInterpolationSegment()
    {
        Vector3 result = QuadraticBezierMath.SampleTangentDeCasteljau(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(4f, 3f, 0f), result);
    }

    [Test]
    public void BernsteinQuadratic_SamplesPointFromEquivalentQuadraticFormula()
    {
        Vector3 result = QuadraticBezierMath.SamplePointBernstein(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(1f, 1.125f, 0f), result);
    }

    [Test]
    public void BernsteinQuadratic_SamplesTangentFromDerivativeFormula()
    {
        Vector3 result = QuadraticBezierMath.SampleTangentBernstein(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(4f, 3f, 0f), result);
    }

    [Test]
    public void PowerBasisQuadratic_SamplesPointFromEquivalentQuadraticFormula()
    {
        QuadraticBezierMath.ComputePowerBasisCoefficients(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            out Vector3 c0, out Vector3 c1, out Vector3 c2);

        Vector3 result = QuadraticBezierMath.SamplePointPowerBasis(c0, c1, c2, 0.25f);

        AssertVector3(new Vector3(1f, 1.125f, 0f), result);
    }

    [Test]
    public void PowerBasisQuadratic_SamplesTangentFromDerivativeFormula()
    {
        QuadraticBezierMath.ComputePowerBasisCoefficients(
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            out Vector3 c0, out Vector3 c1, out Vector3 c2);

        Vector3 result = QuadraticBezierMath.SampleTangentPowerBasis(c1, c2, 0.25f);

        AssertVector3(new Vector3(4f, 3f, 0f), result);
    }

    [Test]
    public void DeCasteljauCubic_SamplesPointFromEquivalentCubicFormula()
    {
        Vector3 result = CubicBezierMath.SamplePoint(
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 3f, 0f),
            new Vector3(3f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(0.90625f, 1.6875f, 0f), result);
    }

    [Test]
    public void DeCasteljauCubic_SamplesTangentFromFinalInterpolationSegment()
    {
        Vector3 result = CubicBezierMath.SampleTangent(
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 3f, 0f),
            new Vector3(3f, 3f, 0f),
            new Vector3(4f, 0f, 0f),
            0.25f);

        AssertVector3(new Vector3(4.125f, 4.5f, 0f), result);
    }

    static void AssertVector3(Vector3 expected, Vector3 actual)
    {
        Assert.That(actual.x, Is.EqualTo(expected.x).Within(Tolerance));
        Assert.That(actual.y, Is.EqualTo(expected.y).Within(Tolerance));
        Assert.That(actual.z, Is.EqualTo(expected.z).Within(Tolerance));
    }
}
