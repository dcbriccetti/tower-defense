using UnityEngine;

public class Shell : MonoBehaviour {
    private float firedAtTime;
    public Transform hitEffectPrefab;

    private void Start() {
        firedAtTime = Time.time;
    }

    private void Update() {
        if (Time.time > firedAtTime + 5) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        var he = Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.identity);
        var ps = he.GetComponent<ParticleSystem>();
        var directionBackTowardGun = transform.position - collision.gameObject.transform.position;
        ps.transform.rotation = Quaternion.LookRotation(directionBackTowardGun);
        Destroy(gameObject);
    }
}