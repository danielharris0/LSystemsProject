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

    public override List<Symbol> Produce() {
        return new List<Symbol> {
            new TurtleSymbols.Move(size*5f, size*0.5f, Color.white),
            new TurtleSymbols.Turn(left),
            new TurtleSymbols.Push(),
            new TurtleSymbols.Push(),
            new ClassicFern(size * sizeMult),
            new TurtleSymbols.Pop(),
            new TurtleSymbols.Turn(right),
            new TurtleSymbols.Move(size*5f, size*0.5f, Color.white),
            new TurtleSymbols.Pop(),
            new TurtleSymbols.Turn(right),
            new TurtleSymbols.Move(size*5f, size*0.5f, Color.white),
            new TurtleSymbols.Push(),
            new TurtleSymbols.Turn(right),
            new TurtleSymbols.Move(size*5f, size*0.5f, Color.white),
            new ClassicFern(size * sizeMult),
            new TurtleSymbols.Pop(),
            new TurtleSymbols.Turn(left),
            new ClassicFern(size * sizeMult)
        };
    }

    public override string ToString() {
        return "Fern";
    }
}