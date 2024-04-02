using UnityEngine;
using Word = System.Collections.Generic.List<Module>;

//A context free parametric L-system
//Since we're context free, each Module can just have their own production rules as a (non)deterministic method

//In future we could upgrade to a normal L-system, or a context-free RGG

public class Cut : Terminal { } //Cut module erases all symbols after it in the branch during the interpretation phase

public static class Parser {

    public static bool Right<T>(Word w, int i) where T:Module {
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
                case ContextFreeModule s:
                    Add(s.Produce());
                    break;
                case ContextSensitiveModule s:
                    Context context = new Context(word, i);
                    Add(s.Produce(context));
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
    public static Word Interpret(Word word) {
        Word newWord = new Word();
        StackState<TraversalState> state = new StackState<TraversalState>(new TraversalState());

        for (int i = 0; i < word.Count; i++) {

            Module s = word[i];
            if (s is Cut) {
                //Cut
                while (i < word.Count && !(word[i] is TurtleModules.Pop)) i++;
                i--;
             } else {
                //Parse by turtle
                newWord.Add(s);
                state.Parse(s);
                if (s is ContextFreeQueryModule) ((ContextFreeQueryModule)s).Query(state.stack.Peek());
            }
        }

        return newWord;

    }
}