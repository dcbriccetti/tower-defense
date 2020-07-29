using UnityEngine;

public class Gun : MonoBehaviour {
    private EnemyManager enemyManager;

    private Transform gunBody;
    void Start() {
        gunBody = transform.Find("Body");
    }

    public void setEnemyManager(EnemyManager enemyManager) {
        this.enemyManager = enemyManager;
    }
    
    void Update() {
        if (enemyManager) {
            var closest = enemyManager.ClosestEnemyTo(transform.position, 3);
            gunBody.LookAt(closest == null ? transform : closest);
        }
    }
}
