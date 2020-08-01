using UnityEngine;

public class ShellHitEffect : MonoBehaviour {
    private void Start() {
        Destroy(gameObject, .2f);
    }
}