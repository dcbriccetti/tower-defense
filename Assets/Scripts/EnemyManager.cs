using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private readonly List<Transform> enemies = new List<Transform>();
    private float launchTime;
    public Transform enemyPrefab;
    public Vector3 startPosition;
    public List<Vector2> Waypoints { get; set; }
    private int waveNumber;

    private void Start() {
        launchTime = Time.time;
    }

    private void Update() {
        if (Time.time > launchTime) {
            ++waveNumber;
            StartCoroutine(nameof(LaunchWave));
            launchTime = Time.time + 20;
        }
    }

    private IEnumerator LaunchWave() {
        var numEnemies = waveNumber * 10;
        for (int i = 0; i < numEnemies; i++) {
            var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
            var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
            var enemy = enemyTransform.GetComponent<Enemy>();
            enemy.Waypoints = Waypoints;
            enemy.Manager = this;
            enemies.Add(enemyTransform);
            yield return new WaitForSeconds(.3f);
        }
    }

    public void Destroy(AbstractEnemy enemy) {
        enemies.Remove(enemy.transform);
        Destroy(enemy.gameObject);
    }

    public Transform ClosestEnemyTo(Vector3 position, float within) {
        var nearbyEnemies = enemies.Where(enemy =>
            (position - enemy.position).sqrMagnitude < within * within).ToList();
        if (nearbyEnemies.Count == 0) return null;
        float SqMag(Transform a) => (position - a.position).sqrMagnitude;
        return nearbyEnemies.Aggregate((a, b) => SqMag(a) < SqMag(b) ? a : b);
    }
}