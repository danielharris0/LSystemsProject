using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fern : ContextFreeSymbol {

    private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up); //* Quaternion.AngleAxis(30, Vector3.forward);
    private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up); //* Quaternion.AngleAxis(30, Vector3.forward);
    private static float sizeMult = 0.6f;

    private float size;
    public Fern(float s) { size = s; }

    public override List<ContextFreeSymbol> Produce() {
        return new List<ContextFreeSymbol> {
            new Move(size, size*0.1f),
            new Turn(left),
            new Push(),
            new Push(),
            new Fern(size * sizeMult),
            new Pop(),
            new Turn(right),
            new Move(size, size*0.1f),
            new Pop(),
            new Turn(right),
            new Move(size, size*0.1f),
            new Push(),
            new Turn(right),
            new Move(size, size*0.1f),
            new Fern(size * sizeMult),
            new Pop(),
            new Turn(left),
            new Fern(size * sizeMult)
        };
    }
}
