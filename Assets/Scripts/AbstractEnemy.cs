using System.Collections.Generic;
using UnityEngine;

public class AbstractEnemy : MonoBehaviour {
    [Range(10, 200)] public int health = 100;
    public float speedMetersPerSecond;
    public List<Vector2> Waypoints { get; set; }
    private int iNextWaypoint;
    private readonly EnemyManager enemyManager = EnemyManager.Instance;
    private bool alive = true;
    [Range(1, 30)] public int rotationSpeed = 8;

    private void Start() {
        var w = Waypoints[0];
        transform.LookAt(new Vector3(w.x, transform.position.y, w.y));
    }

    public virtual void Update() {
        if (transform.position.y < -10) { // Sometimes they fall off
            enemyManager.Destroy(this, false);
            return;
        }

        var waypoint = Waypoints[iNextWaypoint];
        var pos = transform.position;
        var waypoint3 = new Vector3(waypoint.x, pos.y, waypoint.y);
        Quaternion rotTo = Quaternion.LookRotation(waypoint3 - pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotTo, rotationSpeed * Time.deltaTime);
        var toWaypoint = waypoint3 - pos;
        if (toWaypoint.sqrMagnitude < 0.1)
            if (++iNextWaypoint == Waypoints.Count)
                enemyManager.Destroy(this, true);

        transform.Translate(transform.forward * (speedMetersPerSecond * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!alive || !collision.gameObject.CompareTag("Projectile")) return;
        var shell = collision.gameObject.GetComponent<Shell>();
        health -= shell.damage;
        if (health > 0) return;
        enemyManager.Destroy(this, false);
        alive = false;
    }
}
