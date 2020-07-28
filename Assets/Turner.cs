using UnityEngine;

public class Turner : MonoBehaviour
{
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + Vector3.up / 6f, Vector3.one / 6f);
    }

    public char direction { get ; set ; }
}
