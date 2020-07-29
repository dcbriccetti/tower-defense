using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private readonly List<Transform> enemies = new List<Transform>();
    private float launchTime;
    public Transform enemyPrefab;
    public Vector3 startPosition;

    private void Start() {
        launchTime = Time.time + 1;
    }

    public void UpdateEnemies() {
        if (Time.time > launchTime) {
            var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
            enemies.Add(Instantiate(enemyPrefab, pos, enemyPrefab.localRotation));
            launchTime = Time.time + 5;
        }
    }

    public Transform ClosestEnemyTo(Vector3 position, float within) {

        var nearbyEnemies = enemies.Where(enemy => 
            (position - enemy.position).sqrMagnitude < within * within).ToList();
        if (nearbyEnemies.Count == 0) return null;
        return nearbyEnemies.Aggregate((a, b) => {
            var d1 = (position - a.position).sqrMagnitude;
            var d2 = (position - b.position).sqrMagnitude;
            return d1 < d2 ? a : b;
        });
    }
}
