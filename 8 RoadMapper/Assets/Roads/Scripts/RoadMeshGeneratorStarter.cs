using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class RoadMeshGeneratorStarter : MonoBehaviour
{
    public SplinePathTemp splinePath;
    public int rungs = 10;
    public float width = 3f;
    public Material material;
    public float textureRepeatLength = 2f;
    public float thickness = 0.3f; //for thickslab 
    public bool rebuildEveryFrame;
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    public float[] bankAngles;

    void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();

        BuildRoadMesh();
    }

    void Update()
    {
        if (rebuildEveryFrame)
            BuildRoadMesh();
    }

    void BuildRoadMesh()
    {
        List<Vector3> verts = new();
        List<Vector2> uvs = new();
        Array.Resize(ref bankAngles, splinePath.SegmentCount + 1);

        for (int i = 0; i <  rungs; i++)
        {
            //get distance to the rung
            float fraction = i / (float)(rungs - 1);
            float distance = splinePath.TotalLength * fraction;

            //use distance to look up u
            float u = splinePath.ParameterAtDistance(distance);

            //use u to get center point
            Vector3 centerPoint = splinePath.SamplePoint(u);
            Vector3 tangent = splinePath.SampleTangent(u);

            int segment = (int)Mathf.Min(u,splinePath.SegmentCount - 1);
            float t = u - segment;
            float bankAngle = Mathf.SmoothStep(bankAngles[segment], bankAngles[segment + 1], t);

            //calculate right direction
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
            right = Quaternion.AngleAxis(bankAngle, tangent) * right;

            float halfWidth = width * 0.5f;
            float lift = Mathf.Sin(bankAngle * Mathf.Deg2Rad) * halfWidth;

            centerPoint += Vector3.up * Mathf.Abs(lift);

            //create the left and right verts
            Vector3 leftVert = centerPoint - right * halfWidth;
            Vector3 rightVert = centerPoint + right * halfWidth;
            Vector3 bottomLVert = leftVert - (Vector3.down * thickness);
            Vector3 bottomRVert = rightVert - (Vector3.down * thickness);

            verts.Add(leftVert);
            verts.Add(rightVert);
            verts.Add(bottomLVert);
            verts.Add(bottomRVert);

            uvs.Add(new Vector2(0f, distance / textureRepeatLength));
            uvs.Add(new Vector2(1f, distance / textureRepeatLength));
            uvs.Add(new Vector2(0f, distance / textureRepeatLength));
            uvs.Add(new Vector2(1f, distance / textureRepeatLength));
        }

        List<int> triangles = new();
        for (int i = 0; i < rungs - 1; i++)
        {
            int curTopLeft = i * 4; //change to 4 to get the vertices matched
            int curTopRight = curTopLeft + 1; // (i * 4)
            int curBottomLeft = curTopLeft + 2;
            int curBottomRight = curTopLeft + 3;

            int topLeft = (i + 1) * 4;
            int topRight = topLeft + 1;
            int bottomLeft = topLeft + 2;
            int bottomRight = topLeft + 3;

            // top surface ((mostly same code))
            triangles.Add(curTopLeft);
            triangles.Add(topLeft);
            triangles.Add(curTopRight);

            triangles.Add(topLeft);
            triangles.Add(topRight);
            triangles.Add(curTopRight);

            // bottom surface ((mostly same code))
            triangles.Add(curBottomLeft);
            triangles.Add(bottomLeft);
            triangles.Add(curBottomRight);

            triangles.Add(bottomLeft);
            triangles.Add(bottomRight);
            triangles.Add(curBottomRight);

            // left side
            triangles.Add(curTopLeft);
            triangles.Add(curBottomLeft);
            triangles.Add(topLeft);

            triangles.Add(topLeft);
            triangles.Add(curBottomLeft);
            triangles.Add(bottomLeft);

            // right side
            triangles.Add(curTopRight);
            triangles.Add(topRight);
            triangles.Add(curBottomRight);

            triangles.Add(topRight);
            triangles.Add(bottomRight);
            triangles.Add(curBottomRight);
        }
        //int[] triangles = { 0, 2, 1, 1, 2, 3 }; snippet from proceduralquadstarter.cs

        // 2        3
        /*   |----|
         *   | || |
         *   | || |
         *   ||  ||
         * 0 |____| 1
         */


        //front wall end needed
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);

        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);


        //last bit of wall
        int lastEnd = (rungs - 1) * 4; // i < rungs - 1

        triangles.Add(lastEnd);
        triangles.Add(lastEnd + 2);
        triangles.Add(lastEnd + 1);

        triangles.Add(lastEnd + 1);
        triangles.Add(lastEnd + 2);
        triangles.Add(lastEnd + 3);

        Mesh mesh = new();
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        _meshFilter.mesh = mesh;
        _meshRenderer.material = material;

    }
}