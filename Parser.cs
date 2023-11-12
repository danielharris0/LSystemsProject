using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A context free parametric L-system
//Since we're context free, each symbol can just have their own production rules as a (non)deterministic method

//In future we could upgrade to a normal L-system, or a context-free RGG

public abstract class ContextFreeSymbol {
    public abstract List<ContextFreeSymbol> Produce();

    public virtual void Turtle(Turtle t) { }
}

public abstract class ContextFreeTerminal : ContextFreeSymbol {
    public override List<ContextFreeSymbol> Produce() {
        return new List<ContextFreeSymbol> { this };
    }

}
public static class Parser {
    public static List<ContextFreeSymbol> Iterate(List<ContextFreeSymbol> word) {
        List<ContextFreeSymbol> newWord = new List<ContextFreeSymbol>();
        for (int i=0; i<word.Count; i++) {
            newWord.AddRange(word[i].Produce());
        }
        return newWord;
    }
}