//The length of a word cannot be known prior to construction
using Word = System.Collections.Generic.List<ContextFreeSymbol>;
using UnityEngine;
public abstract class ContextFreeSymbol {

    public virtual void ResolveQueryParameters() { } // Called during the interpretation parsing phase (before parsing to produce the next word, all 'query modules/symbols' have their query parameters resolved
    public abstract Word Produce();

}

public abstract class Terminal : ContextFreeSymbol {
    public override Word Produce() {
        return new Word { this };
    }

    public virtual bool Apply(TraversalState state) { return false; }
    public virtual bool Apply(GeometryState state) { return false; }
    public virtual bool Apply<T>(StackState<T> state) where T : State { return false; }
    public virtual bool Apply<T1, T2> (CombinedState<T1, T2> state) where T1 : State where T2 : State { return false; }

}
