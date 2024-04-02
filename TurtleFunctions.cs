
using UnityEngine;

public static class TurtleFunctions {
    public static bool StopTubing(GeometryState s) {
        s.tubing = false;
        return true;
    }
    public static bool StartTubing(GeometryState s) {
        s.tubing = true;
        return true;
    }

    public static bool PlaceQuad(GeometryState s, Material spriteMaterial, Quaternion rotation, Vector3 offset) {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject.DestroyImmediate(obj.GetComponent<MeshCollider>());
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        meshRenderer.material = spriteMaterial;

        obj.transform.SetParent(s.parent.transform, false);
        obj.transform.rotation *= rotation;
        obj.transform.position += s.parent.transform.rotation * offset;

        return true;
    }

    public static bool Move(TraversalState s, float distance, float finalRadius, Color finalColour) {
        s.position += s.orientation * Vector3.forward * distance;
        return true;
    }
    public static bool Move<T1, T2>(CombinedState<T1, T2> s, float distance, float finalRadius, Color finalColour) where T1 : State where T2 : State {
        TraversalState s1 = s.s1 as TraversalState;
        GeometryState s2 = s.s2 as GeometryState;

        if (s1 == null || s2 == null) return false;

        Vector3 end = s1.position + s1.orientation * Vector3.forward * distance;

        if (s2.startColour == Color.clear) { //initial state
            s2.startColour = finalColour;
            s2.startRadius = finalRadius;
        }

        if (s2.tubing) {
            GameObject obj = Geometry.CreateCylinderBetweenPoints(s2.parent, s1.position, end, s2.startRadius, finalRadius, s2.startColour, finalColour, s2.cylinderMaterial);
            s2.parent = obj;
        }

        s2.startColour = finalColour;
        s2.startRadius = finalRadius;

        s1.position = end;

        return true;
    }

    public static bool Turn(TraversalState s, Quaternion rotation) {
        s.orientation *= rotation;
        return true;
    }

}
