using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : ContextFreeTerminal {
    public override void Turtle(Turtle t) {
        t.stack.Push(t.state);
        t.state = t.state.Copy();
    }

}

public class Pop : ContextFreeTerminal {
    public override void Turtle(Turtle t) {
        t.state = t.stack.Pop();
    }
}

public class StopTubing : ContextFreeTerminal {
    public override void Turtle(Turtle t) {
        t.state.tubing = false;
    }
}

public class StartTubing : ContextFreeTerminal {
    public override void Turtle(Turtle t) {
        t.state.tubing = true;
    }
}

public class PlaceQuad : ContextFreeTerminal {

    Vector3 offset;
    Quaternion rotation;
    Material spriteMaterial;
    public PlaceQuad(Vector3 o, Quaternion r, Material m) {
        offset = o; rotation = r; spriteMaterial = m;
    }

    public override void Turtle(Turtle t) {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject.DestroyImmediate(obj.GetComponent<MeshCollider>());
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        meshRenderer.material = spriteMaterial;

        obj.transform.SetParent(t.state.parent.transform, false);
        obj.transform.position += offset;
        obj.transform.rotation *= rotation;
    }
}

public class Move : ContextFreeTerminal {
    public float distance;
    public float finalRadius;
    public Color finalColour;
    public Move(float d, float r, Color c) {
        distance = d; finalRadius = r; finalColour = c;
    }
    public override void Turtle(Turtle t) {
        Vector3 newPosition = t.state.position + t.state.orientation * Vector3.forward * distance;

        if (t.state.startColour == Color.clear) { //initial state
            t.state.startColour = finalColour;
            t.state.startRadius = finalRadius;
        }

        if (t.state.tubing) {
            GameObject obj = t.CreateCylinderBetweenPoints(t.state.parent, t.state.position, newPosition, t.state.startRadius, finalRadius, t.state.startColour, finalColour);
            t.state.parent = obj;
        }

        t.state.startColour = finalColour;
        t.state.startRadius = finalRadius;
        t.state.position = newPosition;
    }
}

public class Turn : ContextFreeTerminal {
    public Quaternion rotation;
    public Turn (Quaternion r) { rotation = r; }
    public override void Turtle(Turtle t) {
        t.state.orientation *= rotation;
    }
}



public class TurtleState {
    public Vector3 position;
    public Quaternion orientation = Quaternion.LookRotation(Vector3.up, Vector3.right);
    public float startRadius;
    public Color startColour = Color.clear;
    public bool tubing = true;
    public int tubeMaterialIndex = 0;
    public Material cylinderMaterial = Resources.Load<Material>("Bark/Bark_ParticleShader");

    public GameObject parent;
    public TurtleState Copy() {
        return (TurtleState)this.MemberwiseClone(); //Note: we need to ensure all state members are structs/primitives
    }
}

public class Turtle {

    public GameObject CreateCylinderBetweenPoints(GameObject parent, Vector3 p1, Vector3 p2, float r1, float r2, Color col1, Color col2) {
        GameObject branch = new GameObject();

        float length = (p2 - p1).magnitude;

        /*
        branch.AddComponent<Sway>();
        Sway sway = branch.GetComponent<Sway>();
        sway.amplitude = 5/length;
        sway.frequency = 5.0f/length;
        */

        MeshFilter meshFilter = branch.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = branch.AddComponent<MeshRenderer>();
        meshRenderer.material = state.cylinderMaterial;

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

    public TurtleState state = new TurtleState();
    public Stack<TurtleState> stack = new Stack<TurtleState>();
    public void Draw(GameObject parentObject, Stack<ContextFreeSymbol> word) {
        state.parent = parentObject;
        while (word.Count>0) {
            word.Pop().Turtle(this);
        }
    }

}
