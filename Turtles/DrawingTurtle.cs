using System.Collections.Generic;
using UnityEngine;

public class DrawingTurtle : TraversalTurtle {

    new public class State : TraversalTurtle.State{
        public float startRadius;
        public Color startColour = Color.clear;
        public bool tubing = true;
        public int tubeMaterialIndex = 0;
        public Material cylinderMaterial = Resources.Load<Material>("Bark/Bark_ParticleShader");

        public GameObject parent;
    }

    new public State state = new State();
    new public Stack<State> stack = new Stack<State>();

    public GameObject CreateCylinderBetweenPoints(GameObject parent, Vector3 p1, Vector3 p2, float r1, float r2, Color col1, Color col2) {
        GameObject branch = new GameObject();

        float length = (p2 - p1).magnitude;

        
        branch.AddComponent<Sway>();
        Sway sway = branch.GetComponent<Sway>();
        sway.amplitude = 6.0f / length;
        sway.frequency = 5.0f/length;
        

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

    public void Draw(GameObject parentObject, Stack<ContextFreeSymbol> word) {
        state.parent = parentObject;
        while (word.Count>0) {
            ContextFreeSymbol symbol = word.Pop();
            symbol.ApplyToTurtle(this);
        }
    }
}
