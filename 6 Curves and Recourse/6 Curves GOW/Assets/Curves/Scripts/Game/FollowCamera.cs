using UnityEngine;

/*
 * FollowCamera holds the camera at a fixed offset from a target. Both fields
 * are assigned in the inspector.
 */

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
