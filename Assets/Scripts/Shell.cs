using UnityEngine;

public class Shell : MonoBehaviour {
    private float firedAtTime;
    public Transform hitEffectPrefab;

    void Start() {
        firedAtTime = Time.time;
    }
    void Update() {
        if (Time.time > firedAtTime + 5) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}