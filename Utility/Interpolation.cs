using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Interpolation {
    public static float Linear(float a, float b, float n) {
        return a * (1 - n) + b * n;
    }

    public static float Cosine(float a, float b, float n) {
        float m = (1 - Mathf.Cos(n * Mathf.PI)) * 0.5f;
        return Linear(a, b, m);
    }

}
