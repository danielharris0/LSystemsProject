using UnityEngine;
using Word = System.Collections.Generic.List<ContextFreeSymbol>;

//A context free parametric L-system
//Since we're context free, each symbol can just have their own production rules as a (non)deterministic method

//In future we could upgrade to a normal L-system, or a context-free RGG


public static class Parser {
    public static Word Iterate(Word word) {
        Word newWord = new Word();
        for (int i=0; i<word.Count; i++) {
            newWord.AddRange(word[i].Produce());
        }
        return newWord;
    }
    public static void Print(Word word) {
        string s = "";
        for (int i = 0; i < word.Count; i++) { s += word[i].ToString() + ", "; }
        Debug.Log(s);
    }
    public static void Interpret(Word word) {

        Vector3 pos = Vector3.zero;
        Quaternion orientation = Quaternion.LookRotation(Vector3.up, Vector3.right);

        //for (int i = 0; i < word.Count; i++) {
        //    newWord.AddRange(word[i].Produce());
        //}
    }
}