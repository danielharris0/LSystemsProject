using System.Collections.Generic;
using UnityEngine;

//Performs traversal over a word, using it's defined symbols for move/turn/push/pop
//It does not do any actual drawing: the extended turtle does this

public class TraversalTurtle {

    public class State {
        public Vector3 position = Vector3.zero;
        public Quaternion orientation = Quaternion.LookRotation(Vector3.up, Vector3.right);
        public State Copy() {
            return (State)this.MemberwiseClone(); //Note: we need to ensure all state members are structs/primitives
        }
    }

    public State state = new State();
    public Stack<State> stack = new Stack<State>();
}