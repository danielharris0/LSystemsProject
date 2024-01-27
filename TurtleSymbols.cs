using System.Collections.Generic;
using UnityEngine;

public class TurtleSymbols {

    public class StopTubing2 : Terminal {
        public override void Apply(GeometryState s) {
            s.tubing = false;
        }

        public override string ToString() { return "StopTubing"; }

    }

    public class StopTubing : Terminal {
        public override void Apply(GeometryState s) {
            s.tubing = false;
        }

        public override string ToString() { return "StopTubing"; }

    }

    public class StartTubing : Terminal {
        public override void Apply(GeometryState s) {
            s.tubing = true;
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

        public override void Apply(GeometryState s) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            GameObject.DestroyImmediate(obj.GetComponent<MeshCollider>());
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = spriteMaterial;

            obj.transform.SetParent(s.parent.transform, false);
            obj.transform.rotation *= rotation;
            obj.transform.position += s.parent.transform.rotation * offset;
        }
    }

    public class Push : Terminal {
        public override void Apply<T>(StackState<T> s) {
            s.stack.Push(s.state);
            s.state = (T) s.state.Copy();
        }

        public override string ToString() { return "["; }
    }

    public class Pop : Terminal {
        public override void Apply<T>(StackState<T> s) { s.state = s.stack.Pop(); }

        public override string ToString() { return "]"; }

    }

    public class Move : Terminal {
        public float distance;
        public float finalRadius;
        public Color finalColour;
        
        private Vector3 startPosition;
        private Vector3 endPosition;

        public Move(float d, float r, Color c) {
            distance = d; finalRadius = r; finalColour = c;
        }

        public override string ToString() { return "MoveDrawing"; }

        public override void Apply(TraversalState s) {
            startPosition = s.position; //assuming this is by-value
            s.position += s.orientation * Vector3.forward * distance;
            endPosition = s.position; //assuming this is by-value
        }

        //assuming  traversal before geometry
        public override void Apply(GeometryState s) {
            if (s.startColour == Color.clear) { //initial state
                s.startColour = finalColour;
                s.startRadius = finalRadius;
            }

            if (s.tubing) {
                GameObject obj = Geometry.CreateCylinderBetweenPoints(s.parent, startPosition, endPosition, s.startRadius, finalRadius, s.startColour, finalColour, s.cylinderMaterial);
                s.parent = obj;
            }

            s.startColour = finalColour;
            s.startRadius = finalRadius;
        }

    }

    public class Turn : Terminal {
        public Quaternion rotation;
        public Turn(Quaternion r) { rotation = r; }
        public override string ToString() { return "Turn"; }
        public override void Apply(TraversalState s) {
            s.orientation *= rotation;
        }
    }


}