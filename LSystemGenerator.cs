using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public enum Seed {ClassicFern, GradientTree}

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
        ContextFreeSymbol seed = (ContextFreeSymbol)Activator.CreateInstance(type, new System.Object[] { numIterations - 1});

        Turtle t = new Turtle();
        t.state.parent = gameObject;

        List<ContextFreeSymbol> word = new List<ContextFreeSymbol> { seed };

        //Parse: iteratively expand
        for (int i = 0; i < numIterations; i++) { word = Parser.Iterate(word); }

        //Interpret with turtle: creating tree of objects
        word.Reverse();
        t.Draw(gameObject, new Stack<ContextFreeSymbol>(word.ToArray()));

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