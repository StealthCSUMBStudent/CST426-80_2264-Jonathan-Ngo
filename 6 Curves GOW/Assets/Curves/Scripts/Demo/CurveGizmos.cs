using System;
using UnityEngine;

/*
 * CurveGizmos is the shared Scene-view helper for quadratic and cubic curves.
 * Both curve components pass in their control points and a way to sample the
 * curve. This class is responsible for drawing the markers, control polygon,
 * and curve.
 */

public static class CurveGizmos
{
    public static void Draw(int numSamples, Func<float, Vector3> samplePoint, params Transform[] controlPoints)
    {

        Gizmos.color = Color.white;
        foreach (Transform t in controlPoints)
            Gizmos.DrawWireSphere(t.position, 0.1f);




        Gizmos.color = Color.red;
        Vector3 lastPosition = controlPoints[0].position;
        for (int i = 1; i < controlPoints.Length; i++)
        {
            Gizmos.DrawLine(lastPosition, controlPoints[i].position);
            lastPosition = controlPoints[i].position;
        }



        Gizmos.color = Color.white;
        Vector3 lastSample = controlPoints[0].position;
        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i/ (numSamples - 1);
            Vector3 position = samplePoint(t);
            Gizmos.DrawLine(lastSample, position);
            lastSample = position;
        }
    }
}
