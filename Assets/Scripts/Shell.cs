using UnityEngine;

public class Shell : MonoBehaviour {
    public Transform hitEffectPrefab;

    private void Start() {
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        var directionBackTowardGun = transform.position - collision.gameObject.transform.position;
        Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.LookRotation(directionBackTowardGun));
        Destroy(gameObject);
    }
}