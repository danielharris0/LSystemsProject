using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {

    private Quaternion baseRotation;
    public float amplitude;
    public float frequency;

    private float offset;

    void Start() {
        offset = Random.Range(0.0f, Mathf.PI);
        baseRotation = transform.rotation;
    }

    void Update() {
        transform.rotation = baseRotation * Quaternion.AngleAxis(Mathf.Sin(Time.time * frequency + offset)* amplitude, Vector3.up) * Quaternion.AngleAxis(Mathf.Cos(Time.time * frequency + offset) * amplitude, Vector3.right);
    }
}
