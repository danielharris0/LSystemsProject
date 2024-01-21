using UnityEngine;

public class TurtleSymbols {

    public class StopTubing : ContextFreeTerminal {
        public override void ApplyToTurtle(DrawingTurtle t) {
            t.state.tubing = false;
        }

        public override string ToString() { return "StopTubing"; }

    }

    public class StartTubing : ContextFreeTerminal {
        public override void ApplyToTurtle(DrawingTurtle t) {
            t.state.tubing = true;
        }

        public override string ToString() { return "StartTubing"; }

    }

    public class PlaceQuad : ContextFreeTerminal {

        Vector3 offset;
        Quaternion rotation;
        Material spriteMaterial;

        public override string ToString() { return "PlaceQuad"; }

        public PlaceQuad(Vector3 o, Quaternion r, Material m) {
            offset = o; rotation = r; spriteMaterial = m;
        }

        public override void ApplyToTurtle(DrawingTurtle t) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            GameObject.DestroyImmediate(obj.GetComponent<MeshCollider>());
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = spriteMaterial;

            obj.transform.SetParent(t.state.parent.transform, false);
            obj.transform.rotation *= rotation;
            obj.transform.position += t.state.parent.transform.rotation * offset;
        }
    }

    public class Push : ContextFreeTerminal {
        public override void ApplyToTurtle(TraversalTurtle t) {
            t.stack.Push(t.state);
            t.state = t.state.Copy();
        }
        public override void ApplyToTurtle(DrawingTurtle t) {
            t.stack.Push(t.state);
            t.state = (DrawingTurtle.State) t.state.Copy();
        }
        public override string ToString() { return "["; }
    }

    public class Pop : ContextFreeTerminal {
        public override void ApplyToTurtle(TraversalTurtle t) { t.state = t.stack.Pop(); }
        public override void ApplyToTurtle(DrawingTurtle t) { t.state = t.stack.Pop(); }

        public override string ToString() { return "]"; }

    }

    public class Move : ContextFreeTerminal {
        public float distance;
        public float finalRadius;
        public Color finalColour;
        public Move(float d, float r, Color c) {
            distance = d; finalRadius = r; finalColour = c;
        }

        public override string ToString() { return "Move"; }

        public override void ApplyToTurtle(TraversalTurtle t) {
            t.state.position += t.state.orientation * Vector3.forward * distance;
            Debug.Log("traversal move");
        }

        public override void ApplyToTurtle(DrawingTurtle t) {
            Vector3 endPosition = t.state.position + t.state.orientation * Vector3.forward * distance;

            if (t.state.startColour == Color.clear) { //initial state
                t.state.startColour = finalColour;
                t.state.startRadius = finalRadius;
            }

            if (t.state.tubing) {
                GameObject obj = t.CreateCylinderBetweenPoints(t.state.parent, t.state.position, endPosition, t.state.startRadius, finalRadius, t.state.startColour, finalColour);
                t.state.parent = obj;
            }

            t.state.startColour = finalColour;
            t.state.startRadius = finalRadius;
            t.state.position = endPosition;

            Debug.Log("drawing move");

        }
    }

    public class Turn : ContextFreeTerminal {
        public Quaternion rotation;
        public Turn(Quaternion r) { rotation = r; }

        public override string ToString() { return "Turn"; }
        public override void ApplyToTurtle(TraversalTurtle t) {
            t.state.orientation *= rotation;
        }
        public override void ApplyToTurtle(DrawingTurtle t) {
            t.state.orientation *= rotation;
        }


    }


}