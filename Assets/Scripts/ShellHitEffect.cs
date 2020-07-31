using UnityEngine;

public class ShellHitEffect : MonoBehaviour {
    private float startedTime;
    void Start() {
        startedTime = Time.time;
    }

    void Update() {
        if (Time.time > startedTime + 0.2f) {
            Destroy(gameObject);
        }
    }
}