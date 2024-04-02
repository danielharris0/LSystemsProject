//The length of a word cannot be known prior to construction
using Word = System.Collections.Generic.List<Module>;
using Rule = System.Tuple<System.Collections.Generic.List<Module>, System.Collections.Generic.List<Module>>;
using System;
#nullable enable

public interface Rule {
    public Word Produce();

}

public abstract class Module {
    public virtual bool Apply(TraversalState state) { return false; }
    public virtual bool Apply(GeometryState state) { return false; }
    public virtual bool Apply<T>(StackState<T> state) where T : State { return false; }
    public virtual bool Apply<T1, T2>(CombinedState<T1, T2> state) where T1 : State where T2 : State { return false; }

}

public abstract class ContextFreeModule : Module {
    public abstract Word Produce();
}

public abstract class ContextFreeQueryModule : ContextFreeModule {
    public abstract void Query(TraversalState traversalState);
}

public class Context { //represents a particular context (position) in a word
    private Word word; private int i;
    public Context(Word word, int i) { this.word = word; this.i = i; }
    public Module? Get(int offset) {
        int k = i + offset;
        if (k < 0 || k >= word.Count) return null;
        return word[k];
    }
}

public abstract class ContextSensitiveModule : Module {
    public abstract Word? Produce(Context context);
}

public abstract class Terminal : ContextFreeModule {
    public override Word Produce() {
        return new Word { this };
    }
}
