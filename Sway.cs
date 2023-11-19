using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {

    private Quaternion baseRotation;
    public float amplitude;
    public float frequency;

    private float offset = Random.Range(0.0f, Mathf.PI);

    void Start() {
        baseRotation = transform.rotation;
    }

    void Update() {
        transform.rotation = baseRotation * Quaternion.Euler((Mathf.Sin(Time.time* frequency + offset) -0.5f)* amplitude, (Mathf.Cos(Time.time*frequency + offset) - 0.5f)* amplitude, 0);
    }
}
