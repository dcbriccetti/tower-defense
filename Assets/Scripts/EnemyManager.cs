using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance;
    public Transform enemyPrefab;
    public int secondsBetweenWaves = 10;
    public int numberOfWaves = 10;
    public int WaveNumber { get; private set; }
    public Vector3 startPosition;
    public List<Vector2> Waypoints { private get; set; }
    private int numDestroyed;
    private int numEscaped;
    private readonly List<Transform> enemies = new List<Transform>();
    private Transform enemiesParentObject;
    public float secondsBetweenEnemiesInWave = .3f;
    private Action<EnemiesChangeEvent> changeListener;

    private void Start() {
        Instance = this;
        StartCoroutine(nameof(LaunchWaves));
        enemiesParentObject = transform.Find("/Enemies");
    }

    private IEnumerator LaunchWaves() {
        for (int i = 1; i <= numberOfWaves; i++) {
            WaveNumber = i;
            StartCoroutine(nameof(LaunchWave));
            yield return new WaitForSeconds(secondsBetweenWaves);
        }
    }

    private IEnumerator LaunchWave() {
        var numEnemies = WaveNumber * 10;
        for (int i = 0; i < numEnemies; i++) {
            var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
            var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
            enemyTransform.SetParent(enemiesParentObject);
            var enemy = enemyTransform.GetComponent<Enemy>();
            enemy.Waypoints = Waypoints;
            enemies.Add(enemyTransform);
            yield return new WaitForSeconds(secondsBetweenEnemiesInWave);
        }
    }

    public void Destroy(AbstractEnemy enemy, bool escaped) {
        enemies.Remove(enemy.transform);
        Destroy(enemy.gameObject);
        if (escaped) ++numEscaped; else ++numDestroyed;
        changeListener(new EnemiesChangeEvent(numDestroyed, numEscaped));
    }

    public Transform ClosestEnemyTo(Vector3 position, float within) {
        var nearbyEnemies = enemies.Where(enemy =>
            (position - enemy.position).sqrMagnitude < within * within).ToList();
        if (nearbyEnemies.Count == 0) return null;
        float SqMag(Transform a) => (position - a.position).sqrMagnitude;
        return nearbyEnemies.Aggregate((a, b) => SqMag(a) < SqMag(b) ? a : b);
    }

    public void AddChangeListener(Action<EnemiesChangeEvent> enemyCallback) {
        changeListener = enemyCallback;
    }
}