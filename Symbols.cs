//The length of a word cannot be known prior to construction
using Word = System.Collections.Generic.List<ContextFreeSymbol>;
using UnityEngine;
public abstract class ContextFreeSymbol {
    public abstract Word Produce();

    public virtual void ApplyToTurtle(TraversalTurtle t) {}
    public virtual void ApplyToTurtle(DrawingTurtle t) {}

}

public abstract class ContextFreeTerminal : ContextFreeSymbol {
    public override Word Produce() {
        return new Word { this };
    }

}