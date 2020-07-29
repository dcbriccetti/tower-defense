using UnityEngine;

public class Enemy : MonoBehaviour {
    public LayerMask turnerLayerMask;
    private GameObject lastTurner;
    private Vector3 direction;
    private const string DirectionCodes = "nesw";
    private static readonly Vector3[] Directions = 
        {Vector3.forward, Vector3.right, Vector3.back, Vector3.left};
    private static readonly int[] Angles = {90, 180, 270, 0};

    void Start() {
        direction = Vector3.back;
        RotateTo(2);
    }
    
    void Update() {
        transform.Translate(direction * Time.deltaTime, Space.World);
        TurnAtTurners();
    }

    private void TurnAtTurners() {
        if (Physics.Linecast(transform.position, transform.position + Vector3.down, out RaycastHit hit, turnerLayerMask)) {
            var other = hit.collider.gameObject;
            if (other == lastTurner)
                return;
            var dirIndex = DirectionCodes.IndexOf(other.GetComponent<Turner>().Direction);
            direction = Directions[dirIndex];
            RotateTo(dirIndex);
            lastTurner = other;
        }
    }

    private void RotateTo(int dirIndex) {
        transform.localRotation = Quaternion.Euler(0, Angles[dirIndex], 0);
    }
}
