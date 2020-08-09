using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance;
    public WaveConfig[] waveConfigs;
    public int secondsBetweenWaves = 10;
    public int WaveNumber { get; private set; }
    public Vector3 StartPosition { private get; set; }
    public List<Vector2> Waypoints { private get; set; }
    private readonly List<Transform> enemies = new List<Transform>();
    private Transform enemiesParentObject;
    private Action<EnemiesChangeEvent> changeListener;
    private WaveConfig currentWaveConfig;

    private void Start() {
        Instance = this;
        StartCoroutine(nameof(LaunchWaves));
        enemiesParentObject = transform.Find("/Enemies");
    }

    private IEnumerator LaunchWaves() {
        foreach (var waveConfig in waveConfigs) {
            currentWaveConfig = waveConfig;
            ++WaveNumber;
            StartCoroutine(nameof(LaunchWave));
            changeListener(new WaveStarted());
            yield return new WaitForSeconds(secondsBetweenWaves + waveConfig.numEnemies * waveConfig.secondsBetweenEnemies);
        }

        StartCoroutine(nameof(EndWhenAllEnemiesAreGone));
    }

    private IEnumerator LaunchWave() {
        var wave = currentWaveConfig;
        for (int i = 0; i < wave.numEnemies; i++) {
            var enemyPrefab = wave.enemyPrefabs[0];
            var pos = new Vector3(StartPosition.x, enemyPrefab.position.y, StartPosition.z);
            var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
            enemyTransform.SetParent(enemiesParentObject);
            var enemy = enemyTransform.GetComponent<AbstractEnemy>();
            enemy.Waypoints = Waypoints;
            enemies.Add(enemyTransform);
            yield return new WaitForSeconds(wave.secondsBetweenEnemies);
        }
    }

    public void Destroy(AbstractEnemy enemy, bool escaped) {
        enemies.Remove(enemy.transform);
        Destroy(enemy.gameObject);
        changeListener(escaped ? (EnemiesChangeEvent) new EnemyEscaped() : new EnemyDestroyed());
    }

    private IEnumerator EndWhenAllEnemiesAreGone() {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0) {
            yield return new WaitForSeconds(2);
        }
        yield return new WaitForSeconds(3);
        changeListener(new AllWavesCompleted());
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
