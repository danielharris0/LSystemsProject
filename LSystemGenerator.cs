using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public enum Seed {ClassicFern, GradientTree, NewTree, E, Topiary}

public class LSystemGenerator : MonoBehaviour {
    
    public Seed seedType;
    public int setRandomSeed;
    public bool mergeMeshes;
    public bool randomiseSeed;
    public int numIterations;

    public void Generate() {
        Debug.Log("Generating...");

        if (randomiseSeed) {
            setRandomSeed = UnityEngine.Random.Range(0, 1000);
        }
        UnityEngine.Random.InitState(setRandomSeed);

        //Kill existing children
        while (transform.childCount > 0) {
            UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Type type = Type.GetType(seedType.ToString());
        ContextFreeModule seed = (ContextFreeModule) Activator.CreateInstance(type, new System.Object[] { numIterations - 1});


        List<Module> word = new List<Module> { seed };
        //Parse: iteratively expand
        for (int i = 0; i < numIterations; i++) {
            Debug.Log(i);
            Parser.Print(word);
            word = Parser.Interpret(word); //generate parameters for environ.-sensitive nodes
            word = Parser.Iterate(word);
        }

        //Interpret with turtle: creating tree of objects
          new Turtle(
            new StackState<CombinedState<TraversalState, GeometryState>>(
                new CombinedState<TraversalState, GeometryState>(
                    new TraversalState(),
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
public class MyScriptEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate")) {
            LSystemGenerator script = (LSystemGenerator) target;
            script.Generate();
        }
    }
}