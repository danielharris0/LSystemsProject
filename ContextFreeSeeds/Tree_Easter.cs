using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E : ContextFreeQueryModule {
    //Constants
    private static Quaternion left = Quaternion.AngleAxis(-25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static Quaternion right = Quaternion.AngleAxis(25, Vector3.up) * Quaternion.AngleAxis(30, Vector3.forward);
    private static Material leafMaterial = Resources.Load<Material>("Leaf/leafMaterial");

    //Meta-parameters (set once at start of generation)
    private int maxAge = 10;

    //Parameters
    private int age = 0;

    //Query Parameters - set to the recieved output from the comms with the environment
    private bool prune;

    public E(int age, int maxAge) { this.age = age; this.maxAge = maxAge; }
    public E(int numIterations) { this.age = 0; this.maxAge = numIterations; }

    public override void Query(TraversalState traversalState) {
        prune = Environment.Query(traversalState, this);
    }


    public override List<Module> Produce() {

        //Since the production ruleset is context-free, we can write the rules IMPERATIVELY



        //Conditional Production
        if (prune) { //Prune
            return new List<Module> {
            };
        } else if (age == maxAge) {
            return new List<Module> {
               // new Cut(),
               // GetMoveModule(),
               // new TurtleModules.PlaceQuad(Vector3.forward*0.6f, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward), leafMaterial)
            };


            //Conditional Production
        } else {
            return new List<Module> {
                 new TurtleModules.Push(),
                 new TurtleModules.Turn(Quaternion.Lerp(Quaternion.identity, Random.rotationUniform, (1 - age/maxAge) * 0.4f)), //small random rotation
                 GetMoveModule(),
                 new E(age+1, maxAge),
                 new E(age+1, maxAge),
                 new TurtleModules.Pop()
            };
        }
    }

    private TurtleModules.Move GetMoveModule() {
        float n = (float) age / (float) maxAge;
        float length = Interpolation.Linear(4f, 0.5f, n);
        float radius = Interpolation.Linear(0.4f, 0.00005f, n); //Radius is interpolated linearly, so individual areas are exponential - but this is countered by the exponenial growth in the number of twigs, so total 'wood mass' is constant for each generation
        return new TurtleModules.Move(length, radius, Color.white);
    }
}
