using System.Collections.Generic;
using UnityEngine;


public abstract class State {
    public abstract void Parse(Module t);
    public virtual State Copy() {
        return (State)this.MemberwiseClone(); //Note: we need to ensure all state members are structs/primitives
    }
}

//Performs traversal over a word, using its defined Modules for move/turn/push/pop
//It does not do any actual drawing: the extended turtle does this
public class TraversalState : State {
    public Vector3 position = Vector3.zero;
    public Quaternion orientation = Quaternion.LookRotation(Vector3.up, Vector3.right);
    public override void Parse(Module t) { t.Apply(this); }

    public TraversalState(Vector3 position) { this.position = position; }
}

public class GeometryState : State {
    public float startRadius;
    public Color startColour = Color.clear;
    public bool tubing = true;
    public int tubeMaterialIndex = 0;
    public Material cylinderMaterial = Resources.Load<Material>("Bark/Bark_ParticleShader");

    public GameObject parent;

    public override void Parse(Module t) { t.Apply(this); }
    public GeometryState(GameObject parent) { this.parent = parent; }

}

public class CombinedState<T1, T2> : State where T1 : State where T2 : State {
    public T1 s1;
    public  T2 s2;
    public CombinedState(T1 s1, T2 s2) { this.s1 = s1; this.s2 = s2; }
    public override void Parse(Module t) {
        if (!t.Apply(this)) {
            s1.Parse(t);
            s2.Parse(t);
        }
    }
    public override State Copy() { return new CombinedState<T1, T2>((T1)s1.Copy(), (T2)s2.Copy()); }
}

public class StackState<T> : State where T : State {
    public Stack<T> stack = new Stack<T>();
    public StackState(T state) { stack.Push(state); }
    public override void Parse(Module t) {
        if (!t.Apply(this)) stack.Peek().Parse(t);
    }
}

class Turtle {
    State state;
    public Turtle(State s) { state = s; }
    public void Parse(List<Module> word) {
        foreach (Module Module in word) {
            state.Parse(Module);
        }
    }
}


