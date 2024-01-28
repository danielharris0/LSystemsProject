using System.Collections.Generic;
using UnityEngine;

public class TurtleSymbols {

    public class StopTubing2 : Terminal {
        public override bool Apply(GeometryState s) {
            s.tubing = false;
            return true;
        }

        public override string ToString() { return "StopTubing"; }

    }

    public class StopTubing : Terminal {
        public override bool Apply(GeometryState s) {
            s.tubing = false;
            return true;
        }

        public override string ToString() { return "StopTubing"; }

    }

    public class StartTubing : Terminal {
        public override bool Apply(GeometryState s) {
            s.tubing = true;
            return true;
        }

        public override string ToString() { return "StartTubing"; }

    }

    public class PlaceQuad : Terminal {

        Vector3 offset;
        Quaternion rotation;
        Material spriteMaterial;

        public override string ToString() { return "PlaceQuad"; }

        public PlaceQuad(Vector3 o, Quaternion r, Material m) {
            offset = o; rotation = r; spriteMaterial = m;
        }

        public override bool Apply(GeometryState s) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            GameObject.DestroyImmediate(obj.GetComponent<MeshCollider>());
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = spriteMaterial;

            obj.transform.SetParent(s.parent.transform, false);
            obj.transform.rotation *= rotation;
            obj.transform.position += s.parent.transform.rotation * offset;

            return true;
        }
    }

    public class Push : Terminal {
        public override bool Apply<T>(StackState<T> s) {
            s.stack.Push(s.state);
            s.state = (T) s.state.Copy();
            return true;
        }

        public override string ToString() { return "["; }
    }

    public class Pop : Terminal {
        public override bool Apply<T>(StackState<T> s) { s.state = s.stack.Pop(); return true; }

        public override string ToString() { return "]"; }

    }

    public class Move : Terminal {
        public float distance;
        public float finalRadius;
        public Color finalColour;

        public Move(float d, float r, Color c) {
            distance = d; finalRadius = r; finalColour = c;
        }

        public override string ToString() { return "MoveDrawing"; }

        public override bool Apply(TraversalState s) {
            s.position += s.orientation * Vector3.forward * distance;
            return true;
        }

        public override bool Apply<T1, T2> (CombinedState<T1, T2> s) {

            TraversalState s1 = s.s1 as TraversalState;
            GeometryState s2 = s.s2 as GeometryState;

            if (s1==null || s2==null) return false;

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

    }

    public class Turn : Terminal {
        public Quaternion rotation;
        public Turn(Quaternion r) { rotation = r; }
        public override string ToString() { return "Turn"; }
        public override bool Apply(TraversalState s) {
            s.orientation *= rotation;
            return true;
        }
    }


}