using System.Collections.Generic;
using UnityEngine;

namespace CollidingTree2 {

    public static class Constants {
        //Constant
        public const float phi = 90;
        public const float alpha = 32;
        public const float beta = 20;

        public const int a = 0;
        public const int b = 1;
        public const int c = -5;

        public const float length = 3f;
        public const float radius = 1f;

        public static Material leafMaterial = Resources.Load<Material>("Leaf/leafMaterial");
    }

    public class Axiom : ContextFreeModule {
        public Axiom(int numIterations) { }

        public override List<Module> Produce() {
            return new List<Module> {
            new Move(1),
            new A(1),
            new P()
        };
        }
    }

    public class T : ContextFreeModule { //T has the identity production
        public override List<Module> Produce() {
            return new List<Module> { new T() };
        }
    }

    public class S : ContextFreeModule { //Signal decays to empty
        public override List<Module> Produce() {
            return new List<Module> { };
        }
    }

    public class P : ContextFreeQueryModule {

        //Query parameter
        public bool prune;

        public override void Query(TraversalState traversalState) {
            prune = Environment.Query(traversalState, this);
        }

        public override List<Module> Produce() {
            return new List<Module> { new P() };
        }
    }

    public class Move : ContextSensitiveModule {

        //Age
        private int k;
        private Module forward;

        public Move(int k) {
            this.k = k;
            float shade = Mathf.Max(1 - 1 / k, 0.2f);
            forward = new TurtleModules.Move(Constants.length, Constants.radius * Mathf.Pow(2,-k/2), new Color(shade,shade,shade));
        }

        public override bool Apply(TraversalState s) { return forward.Apply(s); }
        public override bool Apply<T1, T2>(CombinedState<T1, T2> s) { return forward.Apply(s); }

        public override List<Module> Produce(Context context) {
            Module right = context.Get(1); //Context-sensitive pattern-matching
            if (right != null) {
                if (right is T) return new List<Module> { new S() };
                else if (right is S) return new List<Module> { new S(), new Move(k) };
            }
            return new List<Module> { new Move(k) }; //default identity production
        }
    }

    //Assuming we also need to prove a custom Turn module that propogates S
    public class Turn : ContextSensitiveModule {
        private Quaternion r; public Turn(Quaternion r) { this.r = r; }
        public override bool Apply(TraversalState s) { return new TurtleModules.Turn(r).Apply(s); }

        public override List<Module> Produce(Context context) {
            Module right = context.Get(1); //Context-sensitive pattern-matching
            if (right is S) return new List<Module> { new S(), new Turn(r) };
            return new List<Module> { new Turn(r) }; //default identity production
        }
    }

    //Apex (terminal branch segment)
    public class A : ContextSensitiveModule {

        //Parameter
        private int k = 0;

        public A(int k) { this.k = k; }

        public override List<Module> Produce(Context context) {

            Module right = context.Get(1); //Context-sensitive pattern-matching
            if (right != null && right is P) {
                if (!((P)right).prune) {

                    //Conditional production
                    float p = Mathf.Min(1f, (2 * k + 1) / Mathf.Pow(k, 2)); //Nondeterministic production
                    if (Random.Range(0f, 1f) <= p) {
                        //BIFURCATION: initially exponential, then gets less likely as the branches age
                        return new List<Module> {
                        new Turn(Quaternion.AngleAxis(Constants.phi, Vector3.forward)),
                        new TurtleModules.Push(),
                        new Turn(Quaternion.AngleAxis(Constants.alpha, Vector3.up)),
                        new Move(k),
                        new A(k+1),
                        new P(),
                        new TurtleModules.Pop(),
                        new Turn(Quaternion.AngleAxis(-Constants.beta, Vector3.up)),
                        new Move(k),
                        new A(k+1)
                    };
                    } else {
                        return new List<Module> {
                        new Turn(Quaternion.AngleAxis(Constants.phi, Vector3.forward)),
                        new B(k+1, k+1),
                        new Turn(Quaternion.AngleAxis(-Constants.beta, Vector3.up)),
                        new Move(k),
                        new A(k+1)
                    };
                    }

                } else {
                    //Conditional production
                    return new List<Module> {
                    new T(),
                    //Constants.Leaf(),
                    new Cut()
                };
                }
            }

            Debug.Assert(false); return new List<Module> { };
        }
    }

    //Bud
    public class B : ContextSensitiveModule {

        //Parameter
        private int n = 0; private int m = 0;

        public B(int m, int n) { this.n = n; this.m = m; }

        /*public override bool Apply(GeometryState s) {
            return new TurtleModules.PlaceQuad(Vector3.forward * 0.6f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(Random.Range(-20,20), Vector3.forward), Constants.leafMaterial).Apply(s);
        }*/

        public override List<Module> Produce(Context context) {
            Module right = context.Get(1); //Context-sensitive pattern-matching
            Debug.Assert(right != null);
            if (right is S) {
                //Debug.Log("Bud recieved signal");
                return new List<Module> {
                new TurtleModules.Push(),
                new Turn(Quaternion.AngleAxis(Constants.alpha, Vector3.up)),
                new Move(n),
                new A(Constants.a * m + Constants.b * n + Constants.c),
                new P(),
                new TurtleModules.Pop()
            };
            } else if (right is Move) {
                return new List<Module> {
                new B(m+1,n)
            };
            }
            return new List<Module> { new B(m, n) }; //default identity production
        }
    }

}