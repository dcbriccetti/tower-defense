using UnityEngine;

public class Node : MonoBehaviour
{
    public Transform gunPrefab;
    private Transform gun;
    private Transform guns;

    private void Start() {
        guns = transform.Find("/Guns");
    }

    private void OnMouseDown() {
        if (gun == null) {
            AddGun();
        } else {
            Destroy(gun.gameObject);
            gun = null;
        }
    }

    private void AddGun() {
        Instantiate(gunPrefab, transform.position, Quaternion.identity).SetParent(guns);
    }
}
