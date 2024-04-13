using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public enum Seed {ClassicFern, GradientTree, NewTree, Topiary, CollidingTree, Fern, Conifer }

public class LSystemGenerator : MonoBehaviour {

    public ObstacleManager obstacleManager;

    public Seed lSystem;
    public int seed;
    public bool mergeMeshes;
    public bool randomiseSeed;
    public int numIterations;

    public void Generate() {
        Debug.Log("Generating...");

        if (randomiseSeed) {
            seed = UnityEngine.Random.Range(0, 1000);
        }
        UnityEngine.Random.InitState(seed);

        //Kill existing children
        while (transform.childCount > 0) {
            UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Type type = Type.GetType(lSystem.ToString() + ".Axiom");
        ContextFreeModule axiom = (ContextFreeModule) Activator.CreateInstance(type, new System.Object[] { numIterations - 1});


        List<Module> word = new List<Module> { axiom };
        //Parse: iteratively expand
        for (int i = 0; i < numIterations; i++) {
            word = Parser.Interpret(word, transform.position); //generate parameters for environ.-sensitive nodes
            word = Parser.Iterate(word);
            Parser.Print(word);
        }

        //Interpret with turtle: creating tree of objects
          new Turtle(
            new StackState<CombinedState<TraversalState, GeometryState>>(
                new CombinedState<TraversalState, GeometryState>(
                    new TraversalState(transform.position),
                    new GeometryState(gameObject)
        )))
            .Parse(word);


        //Post-Ops on the created tree
        if (mergeMeshes) { MeshMerger.MergeMeshes(gameObject, Resources.Load<Material>("Bark/Bark_ParticleShader")); }

    }

    void Start() {
        Generate();
    }

}

[CustomEditor(typeof(LSystemGenerator))]
public class LSystemEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate")) {
            Environment.LoadObstacles(((LSystemGenerator) target).obstacleManager);
            LSystemGenerator script = (LSystemGenerator) target;
            script.Generate();
        }
    }
}