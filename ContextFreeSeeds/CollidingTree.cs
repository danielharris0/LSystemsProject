using System.Collections.Generic;
using UnityEngine;



namespace CollidingTree {

    public static class Constants {
        public const float phi = 90;
        public const float alpha = 32;
        public const float beta = 20;

        public const float length = 3f;
        public const float radius = 0.1f;

        public static Material leafMaterial = Resources.Load<Material>("Leaf/leafMaterial2");
        public static TurtleModules.Move GetMove(int age, int maxAge) {
            float n = (float) age / (float) maxAge;
            //Radius motivation: Da Vinci: https://prism.ucalgary.ca/server/api/core/bitstreams/407e4fd6-92f5-494f-bc98-85c8bfcda65c/content
            return new TurtleModules.Move(Constants.length, Interpolation.Linear(2f, 0.00005f, Mathf.Pow(n, 0.25f)) , Color.white);
        }
    }

    public class Axiom : ContextFreeModule {
        private int maxAge;
        public Axiom(int numIterations) { maxAge = 14; }

        public override List<Module> Produce() {
            return new List<Module> {
                new Apex(1, maxAge),
            };
        }
    }

    public class Apex : ContextFreeQueryModule {
        //Parameter
        private int age;
        private bool prune;
        private int maxAge; //(constant)

        public Apex(int age, int maxAge) { this.age = age; this.maxAge = maxAge; }

        public override void Query(TraversalState traversalState) {
            prune = Environment.Query(traversalState, this);
        }


        public override bool Apply(TraversalState s) { return Constants.GetMove(age,maxAge).Apply(s); }
        public override bool Apply<T1, T2>(CombinedState<T1, T2> s) { return Constants.GetMove(age, maxAge).Apply(s); }

        public override List<Module> Produce() {
            if (prune) {
                return new List<Module> {
                    new TurtleModules.Turn(Quaternion.Lerp(Quaternion.identity, Random.rotationUniform, 0.2f)),
                    new Apex(age, maxAge)
                    //new TurtleModules.PlaceQuad(Vector3.forward*3f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward), Constants.leafMaterial)
                };
            } else if (age >= maxAge) {
                return new List<Module> {
                    new TurtleModules.PlaceQuad(Vector3.forward*3f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward), Constants.leafMaterial, 3)
                };
            } else {
                return new List<Module> {
                    Constants.GetMove(age,maxAge),

                    new TurtleModules.Turn(Quaternion.AngleAxis(Constants.phi, Vector3.forward)),

                    new TurtleModules.Push(),
                        new TurtleModules.Turn(Quaternion.AngleAxis(Constants.alpha, Vector3.up)),
                        new Apex(age+2, maxAge), //LATERAL apex (higher age, dies sooner)
                    new TurtleModules.Pop(),

                    new TurtleModules.Turn(Quaternion.AngleAxis(-Constants.beta, Vector3.up)),
                    new Apex(age+1, maxAge) //First-order branch
                };
            }
        }
    }
}