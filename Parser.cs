using UnityEngine;
using Word = System.Collections.Generic.List<Symbol>;

//A context free parametric L-system
//Since we're context free, each symbol can just have their own production rules as a (non)deterministic method

//In future we could upgrade to a normal L-system, or a context-free RGG


public static class Parser {

    public static bool Right<T>(Word w, int i) where T:Symbol {
        return i + 1 < w.Count && w[i + 1] is T;
    }
    public static Word Iterate(Word word) {
        Word newWord = new Word();
        for (int i=0; i<word.Count; i++) {

            void Add(Word w) {
                if (w == null) return;
                newWord.AddRange(w);
            }

            switch (word[i]) {
                case ContextFreeSymbol s:
                    Add(s.Produce());
                    break;
                case ContextSensitiveSymbol s:
                    Add(s.Produce(word, i));
                    break;
            }
        }
        return newWord;
    }
    public static void Print(Word word) {
        string s = "";
        for (int i = 0; i < word.Count; i++) { s += word[i].ToString() + ", "; }
        Debug.Log(s);   
    }
    public static void Interpret(Word word) {

        StackState<TraversalState> state = new StackState<TraversalState>(new TraversalState());

        for (int i = 0; i < word.Count; i++) {
            Symbol s = word[i];
            if (s is Cut) {
                //Cut
                while (!(word[i] is TurtleSymbols.Pop)) i++; i--;
            } else {
                //Parse by turtle
                state.Parse(s);
                s.ResolveQueryParameters(state);
            }
        }
    }
}