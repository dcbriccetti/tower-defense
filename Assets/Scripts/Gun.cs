using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform shellPrefab;
    private EnemyManager enemyManager;

    private Transform gunBody;
    private Transform firePoint;
    private float nextFireTime;

    void Start() {
        nextFireTime = Time.time;
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel").Find("Fire Point");
    }

    public void setEnemyManager(EnemyManager enemyManager) {
        this.enemyManager = enemyManager;
    }

    void Update() {
        if (enemyManager) {
            var closest = enemyManager.ClosestEnemyTo(transform.position, 3);
            if (closest != null) {
                gunBody.LookAt(closest);
                FireWhenReady();
            } else {
                gunBody.rotation = Quaternion.identity;
            }
        }
    }

    private void FireWhenReady() {
        if (Time.time > nextFireTime) {
            var shellRotation = gunBody.rotation;
            var shell = Instantiate(shellPrefab, firePoint.position, shellRotation);
            shell.Rotate(Vector3.right, 90);
            nextFireTime = Time.time + 2;
        }
    }
}