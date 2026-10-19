using UnityEngine;

public class TestBetterSpring : MonoBehaviour
{
    [Header("Scene References")]
    public Transform equilibriumTransform;
    public Transform attachmentTransform;
    public LineRenderer springCoilLine;

    [Header("Spring Settings")]
    public float mass = 1f;
    public float frequencyHz = 1f;
    public float dampingRatio = 0.1f;

    [Header("Debug")]
    public LineRenderer maxLine;
    public LineRenderer minLine;
    public float maxDisplacement;
    float _maxX, _minX;
    float _stepTime;

    const float FixedStep = 1f / 120f;

    void Update()
    {
        // Step the spring in fixed 1/120 s chunks, however long the frame was.
        /*
        _stepTime += Time.deltaTime;
        while (_stepTime >= FixedStep)
        {
            SimulateSpring(FixedStep);
            _stepTime -= FixedStep;
        }
        */
        SimulateSpring(Time.deltaTime);
        UpdateVisuals();
    }

    void SimulateSpring(float deltaTime)
    {
        Vector3 currentPos = attachmentTransform.position;

        // TODO: Turn frequencyHz into a stiffness for this mass, so changing the mass does not
        // change the motion.
        float omega = 2f * Mathf.PI * frequencyHz;
        float stiffness = mass * omega * omega;

        // TODO: Turn dampingRatio into a damping, as a fraction of critical damping.
        // Treat a negative damping ratio as 0.


        // TODO: Add the restoring and damping forces, find the acceleration, and take a
        // semi-implicit Euler step. Keep the velocity in a private _velocity field.
        

        // attachmentTransform.position = ;
    }

    void UpdateVisuals()
    {
        maxLine.positionCount = minLine.positionCount = 2;
        springCoilLine.SetPosition(0, equilibriumTransform.position);
        springCoilLine.SetPosition(1, attachmentTransform.position);

        Vector3 currentPos = attachmentTransform.position;
        Vector3 displacement = currentPos - equilibriumTransform.position;
        maxDisplacement = Mathf.Max(maxDisplacement, displacement.magnitude);

        if (currentPos.x > _maxX)
        {
            _maxX = currentPos.x;
            maxLine.SetPosition(0, currentPos + Vector3.up * 10f);
            maxLine.SetPosition(1, currentPos + Vector3.down * 10f);
        }

        if (currentPos.x < _minX)
        {
            _minX = currentPos.x;
            minLine.SetPosition(0, currentPos + Vector3.up * 10f);
            minLine.SetPosition(1, currentPos + Vector3.down * 10f);
        }
    }
}
