using System.Collections.Generic;
using UnityEngine;

public static class Geometry {
    public static GameObject CreateCylinderBetweenPoints(GameObject parent, Vector3 p1, Vector3 p2, float r1, float r2, Color col1, Color col2, Material cylinderMaterial) {
        GameObject branch = new GameObject();

        float length = (p2 - p1).magnitude;


        branch.AddComponent<Sway>();
        Sway sway = branch.GetComponent<Sway>();
        sway.amplitude = 6.0f / length;
        sway.frequency = 5.0f / length;


        MeshFilter meshFilter = branch.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = branch.AddComponent<MeshRenderer>();
        meshRenderer.material = cylinderMaterial;

        MeshBuilder.TubeOptions options = new MeshBuilder.TubeOptions();
        options.hasColours = true;
        options.colA = col1;
        options.colB = col2;

        meshFilter.mesh = MeshBuilder.Tube(length, 12, r1, r2, options);

        branch.transform.position = p1;
        branch.transform.LookAt(p2);

        Vector3 p = branch.transform.position;
        Vector3 s = branch.transform.localScale;
        Quaternion r = branch.transform.rotation;

        branch.transform.SetParent(parent.transform, true);

        return branch;
    }
}