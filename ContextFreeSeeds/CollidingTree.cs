using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word = System.Collections.Generic.List<Symbol>;

#nullable enable

public static class PruningFunctions {
    public static int L = 10;
    //Prunes to LxLxL cube centered at the origin
    public static bool Prune(Vector3 p) =>  p.x < -L / 2 || p.x > L / 2 || p.y < -L / 2 || p.y > L / 2 || p.z < -L / 2 || p.z > L / 2;
}

public class F : ContextSensitiveSymbol { //Internode F
    public override Word? Produce(Word context, int i) {
        if (Parser.Right<T>(context, i)) return new List<Symbol> { new S() };
        if (Parser.Right<S>(context, i)) return new List<Symbol> { new S(), new F() }; //propogates S basipetally (down)

        return null;
    }

    public override bool Apply<T1, T2>(CombinedState<T1, T2> state) {
        return TurtleFunctions.Move<T1, T2>(state, 1, 1, Color.white);
    }

    public override string ToString() { return "F"; }
}

public class A : ContextSensitiveSymbol { //Apex A
    public override Word? Produce(Word context, int i) {
        if (Parser.Right<P>(context, i) && !PruningFunctions.Prune(((P) context[i + 1]).position.Get())) return new List<Symbol> {
            new O(),
            new F(),
            new TurtleSymbols.Turn(Quaternion.AngleAxis(-180, Vector3.up)),
            new A()
        };
        if (Parser.Right<P>(context, i) && PruningFunctions.Prune(((P)context[i + 1]).position.Get())) return new List<Symbol> {
            new T(),
            new Cut(),
        };
        return null;
    }
    public override string ToString() { return "A"; }
}

public class O : ContextSensitiveSymbol { //Dormant bud
    public override Word? Produce(Word context, int i) {
        if (Parser.Right<S>(context, i)) return new List<Symbol> {
            new TurtleSymbols.Push(),
            new TurtleSymbols.Turn(Quaternion.AngleAxis(-25, Vector3.up)),
            new F(),
            new A(),
            new P(),
            new TurtleSymbols.Pop()
    };
        return null;
    }
    public override string ToString() { return "@o"; }
}

public class S : ContextFreeSymbol { //Bud-activating signal S
    public override string ToString() { return "S"; }
    public override Word Produce() { return new Word(); } //goes to the empty word
}

public class T : Terminal { //pruning signal T
    public override string ToString() { return "T"; }
}

public class Cut : Terminal { //SPECIAL SYMBOL, immediately once produced the parser deletes symbols up to ]
    public override string ToString() { return "%"; }
}

public class P : Terminal { //?P(x,y) positional query module
    public QueryParameter<Vector3> position = new QueryParameter<Vector3>(); //unresolved parameter

    public override void ResolveQueryParameters(StackState<TraversalState> s) {
        position.Set(s.state.position);
    }

    public override string ToString() {
        if (!position.resolved) return "P(*,*,*)";
        else {
            Vector3 p = position.Get();
            return $"P({p.x},{p.y},{p.z})";
        }
    }
}

public class CollidingTree : ContextFreeSymbol {
    public CollidingTree(int n) { }
    public override List<Symbol> Produce() {
        return new List<Symbol> {
            new F(),
            new A(),
            new P()
        };
    }
}