using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTree : ContextFreeSymbol {

    private static Material leafMaterial = Resources.Load<Material>("Leaf/leaf_texture");

    private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static float sizeMult = 0.6f;

    private int age;
    private int maxAge;
    public GradientTree(int numIterations) { age = 0; maxAge = numIterations; }
    public GradientTree(int a, int n) { age = a; maxAge = n; }

    private Move GetMove() {
        float n = (float) age / (float) maxAge;
        float l = Interpolation.Cosine(4f, 0.5f, n); //Mathf.Lerp(20f, 1f, n);
        //We interpolate AREA linearly
        float r = Interpolation.Linear(0.4f, 0.00005f, n); //Mathf.Lerp(4f, 0.05f, n);
        Color colour = Color.Lerp(new Color(0.84f, 0.58f, 0.25f), new Color(0.58f, 0.81f, 0.51f), n);
        return new Move(l, r, colour);
    }

    public override List<ContextFreeSymbol> Produce() {
        if (age==maxAge) {
            return new List<ContextFreeSymbol> {
                GetMove(),
                new PlaceQuad(Vector3.forward*0.6f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward), leafMaterial)
            };
        } else {
            return new List<ContextFreeSymbol> {
                GetMove(),
                new Turn(left * Quaternion.Slerp(Quaternion.identity, Random.rotationUniform, 0.1f)),
                new Push(),
                new Push(),
                new GradientTree(age+1, maxAge),
                new Pop(),
                new Turn(right),
                GetMove(),
                new Pop(),
                new Turn(right),
                GetMove(),
                new Push(),
                new Turn(right),
                GetMove(),
                new GradientTree(age+1, maxAge),
                new Pop(),
                new Turn(left),
                new GradientTree(age+1, maxAge)
            };
        }
    }
}
