using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewTree {
    public class Axiom : ContextFreeModule {

        private static Material leafMaterial = Resources.Load<Material>("Leaf/leafMaterial");
        private static Material[] materials;

        private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
        private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
        private static float sizeMult = 0.6f;

        private int age;
        private int maxAge;
        public Axiom(int numIterations) {
            Debug.Log("new gradient tree num 1");
            materials = new Material[10];
            for (int i = 0; i < 10; i++) {
                materials[i] = new Material(leafMaterial.shader);
                materials[i].mainTexture = leafMaterial.mainTexture;
                materials[i].color = Color.Lerp(Color.white, Color.yellow, (float)i / 9);
            }
            age = 0; maxAge = numIterations;
        }
        public Axiom(int a, int n) { age = a; maxAge = n; }

        private TurtleModules.Move GetMove() {
            float n = (float)age / (float)maxAge;
            float l = Interpolation.Cosine(4f, 0.5f, n); //Mathf.Lerp(20f, 1f, n);
                                                         //We interpolate AREA linearly
            float r = Interpolation.Linear(0.4f, 0.00005f, n); //Mathf.Lerp(4f, 0.05f, n);
            Color colour = Color.Lerp(new Color(0.84f, 0.58f, 0.25f), new Color(0.58f, 0.81f, 0.51f), n);
            return new TurtleModules.Move(l, r, colour);
        }

        public override List<Module> Produce() {
            if (age == maxAge) {
                return new List<Module> {
                GetMove(),
                new TurtleModules.PlaceQuad(Vector3.forward*0.6f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward), materials[Random.Range(0,5)])
            };
            } else {
                return new List<Module> {
                GetMove(),
                new TurtleModules.Turn(left * Quaternion.Slerp(Quaternion.identity, Random.rotationUniform, 0.1f)),
                new TurtleModules.Push(),
                new TurtleModules.Push(),
                new Axiom(age+1, maxAge),
                new TurtleModules.Pop(),
                new TurtleModules.Turn(right),
                GetMove(),
                new TurtleModules.Pop(),
                new TurtleModules.Turn(right),
                GetMove(),
                new TurtleModules.Push(),
                new TurtleModules.Turn(right),
                GetMove(),
                new Axiom(age+1, maxAge),
                new TurtleModules.Pop(),
                new TurtleModules.Turn(left),
                new Axiom(age+1, maxAge)
            };
            }
        }

        public override string ToString() {
            return "Tree";
        }
    }
}