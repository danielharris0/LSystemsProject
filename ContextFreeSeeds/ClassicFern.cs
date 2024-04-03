using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassicFern {
    public class Axiom : ContextFreeModule {

        private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up);
        private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up);
        private static float sizeMult = 0.6f;

        private float size;

        public Axiom(int numIterations) { size = 1; }
        public Axiom(float s) { size = s; }

        public override List<Module> Produce() {
            return new List<Module> {
            new TurtleModules.Move(size*5f, size*0.5f, Color.white),
            new TurtleModules.Turn(left),
            new TurtleModules.Push(),
            new TurtleModules.Push(),
            new Axiom(size * sizeMult),
            new TurtleModules.Pop(),
            new TurtleModules.Turn(right),
            new TurtleModules.Move(size*5f, size*0.5f, Color.white),
            new TurtleModules.Pop(),
            new TurtleModules.Turn(right),
            new TurtleModules.Move(size*5f, size*0.5f, Color.white),
            new TurtleModules.Push(),
            new TurtleModules.Turn(right),
            new TurtleModules.Move(size*5f, size*0.5f, Color.white),
            new Axiom(size * sizeMult),
            new TurtleModules.Pop(),
            new TurtleModules.Turn(left),
            new Axiom(size * sizeMult)
        };
        }

    }
}