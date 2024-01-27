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

    public virtual void Apply(TraversalState state) {}
    public virtual void Apply(GeometryState state) {}
    public virtual void Apply<T>(StackState<T> state) where T : State {}

}
