//The length of a word cannot be known prior to construction
using Word = System.Collections.Generic.List<Symbol>;
using Rule = System.Tuple<System.Collections.Generic.List<Symbol>, System.Collections.Generic.List<Symbol>>;
#nullable enable

public interface Rule {
    public Word Produce();

}

public abstract class Symbol {
    public virtual void ResolveQueryParameters(StackState<TraversalState> s) { } // Called during the interpretation parsing phase (before parsing to produce the next word, all 'query modules/symbols' have their query parameters resolved
}

public abstract class ContextFreeSymbol : Symbol {
    public abstract Word Produce();

}

public abstract class ContextSensitiveSymbol : Symbol {
    public abstract Word? Produce(Word context, int i);
}

public class QueryParameter<T> {
    private T value;
    public bool resolved = false;
    public T Get() { return value; }
    public void Set(T value) { resolved = true;  this.value = value; }
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
