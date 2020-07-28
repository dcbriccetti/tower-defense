using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject lastTurner;
    private Vector3 direction;
    private const string Directions = "nesw";
    private static readonly Vector3[] Dirs = 
        {Vector3.forward, Vector3.right, Vector3.back, Vector3.left};

    void Start() {
        direction = Vector3.back;
    }
    
    void Update() {
        transform.Translate(direction * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision) {
        var other = collision.gameObject;
        if (other == lastTurner || !other.CompareTag("Turner")) 
            return;
        var turner = other.GetComponent<Turner>();
        direction = Dirs[Directions.IndexOf(turner.direction)];
        lastTurner = other;
    }
}
