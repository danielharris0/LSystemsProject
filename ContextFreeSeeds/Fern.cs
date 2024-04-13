using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fern {
    public class Axiom : ContextFreeModule {

        public Axiom(int numIterations) { }
        public Axiom() { }

        private static Quaternion left = Quaternion.AngleAxis(-20, Vector3.up);
        private static Quaternion right = Quaternion.AngleAxis(20, Vector3.up);

        public override List<Module> Produce() {
            return new List<Module> {
                new F(),
                new TurtleModules.Turn(left),

                new TurtleModules.Push(),
                    new TurtleModules.Push(),
                        new Axiom(),
                    new TurtleModules.Pop(),
                    new TurtleModules.Turn(right),
                    new TurtleModules.Turn(right),
                    new Axiom(),
                new TurtleModules.Pop(),
                new TurtleModules.Turn(right),
                new F(),

                new TurtleModules.Push(),
                    new TurtleModules.Turn(right),
                    new F(),
                    new Axiom(),
                new TurtleModules.Pop(),
                new TurtleModules.Turn(left),
                new Axiom()
            };
        }

    }

    public class F : ContextFreeModule {

        private static Module forward = new TurtleModules.Move(1.2f, 0.5f, Color.white);
        public override bool Apply(TraversalState s) { return forward.Apply(s); }
        public override bool Apply<T1, T2>(CombinedState<T1, T2> s) { return forward.Apply(s); }

        public override List<Module> Produce() {
            return new List<Module> {
                new F(),
                new F()
            };
        }

    }
}