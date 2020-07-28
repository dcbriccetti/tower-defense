using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject lastTurnerGo;
    void Update() {
        transform.Translate(Vector3.forward * (Time.deltaTime * 2f));
    }

    private void OnCollisionEnter(Collision other) {
        var otherGo = other.gameObject;
        if (otherGo != lastTurnerGo && otherGo.CompareTag("Turner")) {
            var turner = otherGo.GetComponent<Turner>();
            int turn = turner.direction == 'l' ? 90 : -90;
            transform.Rotate(0, turn, 0);
            lastTurnerGo = otherGo;
        }
    }
}
