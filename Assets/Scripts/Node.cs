using UnityEngine;

public class Node : MonoBehaviour {
    public Transform gunPrefab;
    public CashManager CashManager { private get; set ; }
    private Transform gun;
    private Transform guns;
    private const int GunCostInDollars = 100;

    private void Start() {
        guns = transform.Find("/Guns");
    }

    private void OnMouseDown() {
        if (gun == null) {
            if (CashManager.Buy(GunCostInDollars)) {
                AddGun();
            }
        } else {
            CashManager.Receive(GunCostInDollars / 2);
            Destroy(gun.gameObject);
            gun = null;
        }
    }

    private void AddGun() {
        gun = Instantiate(gunPrefab, transform.position, Quaternion.identity);
        gun.SetParent(guns);
    }
}