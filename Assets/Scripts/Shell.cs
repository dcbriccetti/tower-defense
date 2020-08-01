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
            var he = Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.identity);
            var ps = he.GetComponent<ParticleSystem>();
            var dir = transform.position - collision.gameObject.transform.position;
            ps.transform.rotation = Quaternion.LookRotation(dir);
            Destroy(gameObject);
        }
    }
}