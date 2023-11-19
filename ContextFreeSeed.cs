using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fern : ContextFreeSymbol {

    private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static float sizeMult = 0.6f;

    private float age;
    private float maxAge;
    public Fern(float a, float n) { age = a; maxAge = n; }

    private Move GetMove() {
        float n = age / maxAge;
        float l = Interpolation.Cosine(4f, 0.5f, n); //Mathf.Lerp(20f, 1f, n);
        //We interpolate AREA linearly
        float r = Interpolation.Linear(0.4f, 0.00005f, n); //Mathf.Lerp(4f, 0.05f, n);
        Debug.Log(r);

        Color colour = Color.Lerp(new Color(0.84f, 0.58f, 0.25f), new Color(0.58f, 0.81f, 0.51f), n);
        return new Move(l, r, colour);
    }

    public override List<ContextFreeSymbol> Produce() {
        return new List<ContextFreeSymbol> {
            GetMove(),
            new Turn(left),
            new Push(),
            new Push(),
            new Fern(age+1, maxAge),
            new Pop(),
            new Turn(right),
            GetMove(),
            new Pop(),
            new Turn(right),
            GetMove(),
            new Push(),
            new Turn(right),
            GetMove(),
            new Fern(age+1, maxAge),
            new Pop(),
            new Turn(left),
            new Fern(age+1, maxAge)
        };
    }
}
