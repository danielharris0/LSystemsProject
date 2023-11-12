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

public class Move : ContextFreeTerminal {
    public float distance;
    public float finalRadius;
    public Color finalColour;
    public Move(float d, float r, Color c) {
        distance = d; finalRadius = r; finalColour = c;
    }
    public override void Turtle(Turtle t) {
        Vector3 newPosition = t.state.position + t.state.orientation * Vector3.forward * distance;

        if (t.state.tubing) {
            t.CreateCylinderBetweenPoints(t.state.position, newPosition, t.state.startRadius, finalRadius, t.state.startColour, finalColour);
        }

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
    public float startRadius = 0.1f;
    public Color startColour = Color.white;
    public bool tubing = true;
    public int tubeMaterialIndex = 0;
    public TurtleState Copy() {
        return (TurtleState)this.MemberwiseClone(); //Note: we need to ensure all state members are structs/primitives
    }
}

public class Turtle : MonoBehaviour {

    public Material[] materials;

    public void CreateCylinderBetweenPoints(Vector3 p1, Vector3 p2, float r1, float r2, Color col1, Color col2) {
        GameObject branch = new GameObject();
        MeshFilter meshFilter = branch.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = branch.AddComponent<MeshRenderer>();
        meshRenderer.material = materials[state.tubeMaterialIndex];
        meshFilter.mesh = MeshBuilder.Tube(true, 6, r1, r2, col1, col2);

        branch.transform.position = p1;
        branch.transform.localScale = new Vector3(1, 1, (p2 - p1).magnitude);
        branch.transform.LookAt(p2);
        branch.transform.parent = gameObject.transform;
    }

    public TurtleState state = new TurtleState();
    public Stack<TurtleState> stack = new Stack<TurtleState>();
    public void Draw(Stack<ContextFreeSymbol> word) {
        while (word.Count>0) {
            word.Pop().Turtle(this);
        }
    }

    public void Start() {
        List<ContextFreeSymbol> word = new List<ContextFreeSymbol>{ new ContextFreeSeed(0) };
        for (int i = 0; i < 6; i++) { word = Parser.Iterate(word); }
        word.Reverse();
        Draw(new Stack<ContextFreeSymbol>(word.ToArray()));
        MeshMerger.MergeMeshes(gameObject,materials[0]);
    }
}
