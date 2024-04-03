using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class Environment {

    private static ObstacleMode obstacleMode;

    public static List<PrimitiveCollider> containers = new List<PrimitiveCollider>();

    private static bool InUnitCube(Vector3 v) {
        return v.x >= 0 && v.x <= 1 && v.y >= 0 && v.y <= 1 && v.z >= 0 && v.z <= 1;
    }

    public static void LoadObstacles(ObstacleManager obstacleManager) {
        obstacleMode = obstacleManager.obstacleMode;
        containers = new List<PrimitiveCollider>();
        foreach (Transform child in obstacleManager.transform) {
            GameObject obj = child.gameObject;
            string name = obj.GetComponent<MeshFilter>().name;
            if (name == "Cube") containers.Add(new CuboidCollider(child.position - child.localScale/2, child.localScale));
            if (name == "Sphere") containers.Add(new SphereCollider(child.position, child.localScale.x/2));
            if (name == "Cylinder") containers.Add(new CylinderCollider(child.position - Vector3.up*child.localScale.y, child.localScale.x / 2, child.localScale.y*2));
        }
    }

    //Environmental Query: recieves turtle TRAVERSAL state and the query module, and returns PARAMETERS to set in the query module

    //Optimisation: make this concurrent to the string rewriting (but probably not neccesary)
    public static bool Query(TraversalState traversalState, ContextFreeQueryModule queryModule) { //temp. return data: a simple pruning function
        if (obstacleMode == ObstacleMode.Contain) {
            foreach (PrimitiveCollider collider in containers) {
                if (collider.Contains(traversalState.position)) return false;
            }
            return true;
        } else {
            foreach (PrimitiveCollider collider in containers) {
                if (collider.Contains(traversalState.position)) return true;
            }
            return false;
        }

    }
}

public abstract class PrimitiveCollider {
    public abstract bool Contains(Vector3 p);
}

public class CuboidCollider : PrimitiveCollider {
    private Vector3 position; private Vector3 size;
    public CuboidCollider(Vector3 position, Vector3 size) { this.position = position; this.size = size; }

    public override bool Contains(Vector3 p) {
        Vector3 v = p - position;
        return v.x >= 0 && v.x <= size.x && v.y >= 0 && v.y <= size.y && v.z >= 0 && v.z <= size.z;
    }
}
public class SphereCollider : PrimitiveCollider {
    private Vector3 position; private float radius;
    public SphereCollider(Vector3 position, float radius) { this.position = position; this.radius = radius;}
    public override bool Contains(Vector3 p) {
        Vector3 v = p - position;
        return v.magnitude <= radius;
    }
}
public class CylinderCollider : PrimitiveCollider {
    private Vector3 position; private float radius; private float height;
    public CylinderCollider(Vector3 position, float radius, float height) { this.position = position; this.radius = radius; this.height = height; }
    public override bool Contains(Vector3 p) {
        Vector3 v = p - position;
        Vector2 u = new Vector2(v.x, v.z);
        return u.magnitude <= radius && v.y >= 0 && v.y <= height;
    }
}