﻿using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform shellPrefab;
    public float fireDelay = 0.4f;
    public EnemyManager EnemyManager { get; private set; }

    private Transform gunBody;
    private Transform firePoint;
    private float nextFireTime;

    void Start() {
        nextFireTime = Time.time;
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel").Find("Fire Point");
    }

    public void setEnemyManager(EnemyManager enemyManager) {
        this.EnemyManager = enemyManager;
    }

    void Update() {
        if (EnemyManager) {
            var closest = EnemyManager.ClosestEnemyTo(transform.position, 3);
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
            nextFireTime = Time.time + fireDelay;
        }
    }
}