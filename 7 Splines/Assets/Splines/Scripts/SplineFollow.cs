using UnityEditor.Experimental.GraphView;
using UnityEngine;

/*
 * SplineFollow rides a SplinePath. Each frame it moves along the path,
 * finds u, places itself on the curve, and faces the target or the tangent.
 */

public class SplineFollow : MonoBehaviour
{
    public SplinePath path;
    public Transform target;
    public float speed = 2.5f; // Positive world units per second in the completed exercise.
    public bool travelByDistance = true;
    public bool faceTarget = true;
    public bool rockChecker = false;

    float _distance;
    float _u;

    void Update()
    {
        if (travelByDistance)
        {
            // TODO: Advance distance by speed over the frame and look up u for that distance.
            // Stop at TotalLength.
            _distance += speed * Time.deltaTime;
            _distance = Mathf.Clamp(_distance, 0f, path.TotalLength);

            if (_distance >= path.TotalLength && rockChecker == false)
            {
                rockChecker = true;
                Debug.Log("Liftoff! => " + rockChecker);
            }


            _u = path.ParameterAtDistance(_distance);
            //rockChecker = true;
            //Debug.Log("We are Ready! Liftoff! => " + rockChecker);
        }
        else
        {
            // TODO: Advance u in equal steps, paced so the trip takes as long as the distance
            // trip at the same speed. Stop at SegmentCount.
            float duration = path.TotalLength / speed;

            _u += path.SegmentCount * Time.deltaTime / duration;
            _u = Mathf.Clamp(_u, 0f, path.SegmentCount);
        }

        // TODO: Place this object at the path point for u. Replay should return it to the start.
        transform.position = path.SamplePoint(_u);

        // TODO: Look at the target if faceTarget is on, otherwise along the path tangent.
        // Use world up so the horizon stays level.
        Vector3 faceDirection;
        if (faceTarget == true)
        {
            faceDirection = target.position - transform.position;
        } else
        {
            faceDirection = path.SampleTangent(_u);
        }

        if (faceDirection.sqrMagnitude != 0)
        {
            Quaternion rotateDirection = Quaternion.LookRotation(faceDirection, Vector3.up);
            transform.rotation = rotateDirection;
        }
    }

    public void Restart()
    {
        _distance = 0f;
        _u = 0f;
        rockChecker = false;
    }
}
