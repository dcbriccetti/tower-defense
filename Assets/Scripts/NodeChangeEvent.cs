using UnityEngine;

public abstract class NodeChangeEvent { }

public class GunAddedToNode : NodeChangeEvent {
    public GameObject gun;

    public GunAddedToNode(GameObject gunGameObject) {
        this.gun = gunGameObject;
    }
}
