using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public Transform enemyPrefab;
    public int secondsBetweenWaves = 10;
    public int numberOfWaves = 10;
    public Vector3 startPosition;
    public List<Vector2> Waypoints { get; set; }
    public static EnemyManager Instance;
    private readonly List<Transform> enemies = new List<Transform>();
    private int waveNumber;

    private void Start() {
        Instance = this;
        StartCoroutine(nameof(LaunchWaves));
    }

    private IEnumerator LaunchWaves() {
        for (waveNumber = 1; waveNumber <= numberOfWaves; ++waveNumber) {
            StartCoroutine(nameof(LaunchWave));
            yield return new WaitForSeconds(secondsBetweenWaves);
        }
    }

    private IEnumerator LaunchWave() {
        var numEnemies = waveNumber * 10;
        for (int i = 0; i < numEnemies; i++) {
            var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
            var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
            var enemy = enemyTransform.GetComponent<Enemy>();
            enemy.Waypoints = Waypoints;
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