using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicFern : ContextFreeSymbol {

    private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up);
    private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up);
    private static float sizeMult = 0.6f;

    private float size;

    public ClassicFern(int numIterations) { size = 1; }
    public ClassicFern(float s) { size = s; }

    public override List<ContextFreeSymbol> Produce() {
        return new List<ContextFreeSymbol> {
            new Move(size*5f, size*0.5f, Color.white),
            new Turn(left),
            new Push(),
            new Push(),
            new ClassicFern(size * sizeMult),
            new Pop(),
            new Turn(right),
            new Move(size*5f, size*0.5f, Color.white),
            new Pop(),
            new Turn(right),
            new Move(size*5f, size*0.5f, Color.white),
            new Push(),
            new Turn(right),
            new Move(size*5f, size*0.5f, Color.white),
            new ClassicFern(size * sizeMult),
            new Pop(),
            new Turn(left),
            new ClassicFern(size * sizeMult)
        };
    }
}