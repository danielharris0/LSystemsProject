using UnityEngine;
using UnityEditor;
using System.Collections.Generic;




public class ObstacleManager : MonoBehaviour { }

[CustomEditor(typeof(ObstacleManager))]
public class ObstacleEditor : Editor {


public override void OnInspectorGUI() {

        void AddPrimitive(PrimitiveType primitiveType) {
            ObstacleManager script = (ObstacleManager) target;
            GameObject obstacle = GameObject.CreatePrimitive(primitiveType);
            obstacle.transform.parent = script.transform;
            obstacle.GetComponent<MeshRenderer>().material = Resources.Load<Material>("ObstacleMaterial");

            float s = 10;
            obstacle.transform.localScale = Vector3.one * s;
            obstacle.transform.position = obstacle.transform.position + Vector3.up * s / 2;

            Selection.activeObject = obstacle;
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Add Cuboid")) { AddPrimitive(PrimitiveType.Cube); }
        if (GUILayout.Button("Add Sphere")) { AddPrimitive(PrimitiveType.Sphere); }
        if (GUILayout.Button("Add Cylinder")) { AddPrimitive(PrimitiveType.Cylinder); }

    }
}