using System.Collections;
using UnityEngine;

public class Shell : MonoBehaviour {
    public Transform hitEffectPrefab;
    public int damage = 100;
    private int hits;

    private void Start() {
        Destroy(gameObject, 60);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        if (++hits == 1) {
            var directionBackTowardGun = transform.position - collision.gameObject.transform.position;
            Instantiate(hitEffectPrefab, collision.transform.position, Quaternion.LookRotation(directionBackTowardGun));
        }

        Destroy(gameObject);
    }
}