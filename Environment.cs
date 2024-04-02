using UnityEngine;

public static class Environment {

    private static bool InUnitCube(Vector3 v) {
        return v.x >= 0 && v.x <= 1 && v.y >= 0 && v.y <= 1 && v.z >= 0 && v.z <= 1;
    }

    //Environmental Query: recieves turtle TRAVERSAL state and the query module, and returns PARAMETERS to set in the query module

    //Optimisation: make this concurrent to the string rewriting (but probably not neccesary)
    public static bool Query(TraversalState traversalState, ContextFreeQueryModule queryModule) { //temp. return data: a simple pruning function
        return !InUnitCube(traversalState.position / 10 + new Vector3(0.5f, 0, 0.5f));
    }
}