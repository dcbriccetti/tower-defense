using UnityEngine;

public class Node : MonoBehaviour
{
    public Transform gunPrefab;
    private Transform gun;

    private void OnMouseDown() {
        if (gun == null) {
            AddGun();
        } else {
            Destroy(gun.gameObject);
            gun = null;
        }
    }

    public void AddGun() {
        gun = Instantiate(gunPrefab, transform.position, Quaternion.identity);
    }
}
