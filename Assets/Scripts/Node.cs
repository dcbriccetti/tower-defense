using System;
using UnityEngine;

public class Node : MonoBehaviour {
    [SerializeField] private Transform[] gunPrefabs;
    public CashManager CashManager { private get; set ; }
    private Transform gun;
    private Transform gunsContainer;
    public Func<bool> IsMouseClickAllowed { get; set; }
    public Action<NodeChangeEvent> NodeChangeListener { get ; set ; }
    public Func<int> SelectedGunIndexProvider { get ; set ; }

    private const int GunCostInDollars = 100;

    private void Start() {
        gunsContainer = transform.Find("/Instance Containers/Guns");
    }

    private void OnMouseDown() {
        if (! IsMouseClickAllowed()) return;
        if (gun == null) {
            if (!CashManager.Buy(GunCostInDollars)) return;
            NodeChangeListener(new GunAddedToNode());
            gun = Instantiate(gunPrefabs[SelectedGunIndexProvider()], transform.position, Quaternion.identity, gunsContainer);
        } else {
            CashManager.Receive(GunCostInDollars / 2);
            Destroy(gun.gameObject);
            gun = null;
        }
    }
}