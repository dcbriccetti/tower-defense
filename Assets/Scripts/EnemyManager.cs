using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager instance;
    [NonSerialized] public LevelConfig[] levelConfigs;
    public int WaveNumber { get; private set; }
    public Vector3 StartPosition { private get; set; }
    public List<Vector2> Waypoints { private get; set; }
    private readonly List<GameObject> enemies = new List<GameObject>();
    private Transform enemiesParentObject;
    public Action<EnemiesChangeEvent> ChangeListener { get; set; }
    private int currentLevelIndex = 0;
    private WaveConfig[] waveConfigs;
    private WaveConfig currentWaveConfig;

    private void Start() {
        instance = this;
        enemiesParentObject = transform.Find("/Instance Containers/Enemies");
    }

    public void StartLevel(int levelIndex) {
        currentLevelIndex = levelIndex;
        waveConfigs = levelConfigs[currentLevelIndex].waveConfigs;
        StartCoroutine(nameof(LaunchWaves));
    }

    private IEnumerator LaunchWaves() {
        for (int i = levelConfigs[currentLevelIndex].startingWaveIndex; i < waveConfigs.Length; i++) {
            var waveConfig = waveConfigs[i];
            currentWaveConfig = waveConfig;
            ++WaveNumber;
            StartCoroutine(nameof(LaunchWave));
            ChangeListener(new WaveStarted());
            yield return new WaitForSeconds(levelConfigs[currentLevelIndex].secondsBetweenWaves + waveConfig.numEnemies * waveConfig.secondsBetweenEnemies);
        }

        StartCoroutine(nameof(EndWhenAllEnemiesAreGone));
    }

    private IEnumerator LaunchWave() {
        var wave = currentWaveConfig;
        for (int i = 0; i < wave.numEnemies; i++) {
            var enemyPrefab = wave.enemyPrefabs[0];
            var pos = new Vector3(StartPosition.x, enemyPrefab.position.y, StartPosition.z);
            var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.rotation, enemiesParentObject);
            var enemy = enemyTransform.GetComponent<Enemy>();
            enemy.Waypoints = Waypoints;
            enemies.Add(enemyTransform.gameObject);
            yield return new WaitForSeconds(wave.secondsBetweenEnemies);
        }
    }

    public void Destroy(Enemy enemy, bool escaped) {
        enemies.Remove(enemy.gameObject);
        Destroy(enemy.gameObject);
        ChangeListener(escaped ? (EnemiesChangeEvent) new EnemyEscaped() : new EnemyDestroyed(enemy.GetComponent<Enemy>())); // todo
    }

    public bool IsAlive(GameObject enemy) => enemies.Contains(enemy);

    private IEnumerator EndWhenAllEnemiesAreGone() {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0) {
            yield return new WaitForSeconds(2);
        }
        yield return new WaitForSeconds(3);
        ChangeListener(new AllWavesCompleted());
    }

    public readonly struct TargetInfo {
        public readonly GameObject enemy;
        public readonly Vector3 futurePos;
        public TargetInfo(GameObject enemy, Vector3 futurePos) {
            this.enemy = enemy;
            this.futurePos = futurePos;
        }
    }
    public TargetInfo? BestEnemyTarget(Transform gunTransform, float within, float secondsInFuture) {
        var targetInfos =
            from enemy in enemies
            let futurePos = enemy.GetComponent<Enemy>().FuturePosition(secondsInFuture)
            let distance = (gunTransform.position - futurePos).magnitude
            let angleBetween = Quaternion.Angle(Quaternion.LookRotation(futurePos - gunTransform.position), gunTransform.rotation)
            where distance < within
            orderby angleBetween / 360f + distance / within
            select new TargetInfo(enemy, futurePos);
        var nearbyEnemies = targetInfos.ToList();
        if (nearbyEnemies.Count == 0) return null;

        return nearbyEnemies[0];
    }
}
