using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private readonly List<Transform> enemies = new List<Transform>();
    private float launchTime;
    public Transform enemyPrefab;
    public Vector3 startPosition;
    public List<Vector2> Waypoints { get; set; }

    private void Start() {
        launchTime = Time.time;
    }

    public void UpdateEnemies() {
        if (!(Time.time > launchTime)) return;
        var pos = new Vector3(startPosition.x, enemyPrefab.position.y, startPosition.z);
        var enemyTransform = Instantiate(enemyPrefab, pos, enemyPrefab.localRotation);
        var enemy = enemyTransform.GetComponent<Enemy>();
        enemy.Waypoints = Waypoints;
        enemy.Manager = this;
        enemies.Add(enemyTransform);
        launchTime = Time.time + 5;
    }

    public void Destroy(Enemy enemy) {
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
