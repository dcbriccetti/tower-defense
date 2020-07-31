using System;
using UnityEngine;

public class Shell : MonoBehaviour {
    public float speedMetersPerSecond;

    void Start() {
    }

    void Update() {
        transform.Translate(transform.up * (speedMetersPerSecond * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
    }
}