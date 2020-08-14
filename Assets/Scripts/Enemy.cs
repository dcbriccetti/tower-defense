using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] [Range(10, 200)] private int health = 100;
    [SerializeField] public int killValue = 5;
    [SerializeField] [Range(0, 20)] private float speedMetersPerSecond = 1;
    [SerializeField] [Range(1, 30)] public int rotationSpeed = 8;
    public List<Vector2> Waypoints { get; set; }
    private int iNextWaypoint;
    private readonly EnemyManager enemyManager = EnemyManager.Instance;
    private bool alive = true;

    private void Start() {
        var w = Waypoints[0];
        transform.LookAt(new Vector3(w.x, transform.position.y, w.y));
    }

    public void FixedUpdate() {
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
        shell.damage = 0; // Shells have been striking twice, somehow
        if (health <= 0) {
            enemyManager.Destroy(this, false);
            alive = false;
        } else {
            var head = transform.Find("Head");
            if (head != null) {
                var animator = head.GetComponent<Animator>();
                if (animator != null)
                    animator.Play("Struck");
            }
        }
    }

    /// <summary>
    /// Where this enemy will be `seconds` seconds from now
    /// </summary>
    /// <param name="seconds">A number of seconds into the future</param>
    /// <returns>Where this enemy will be</returns>
    public Vector3 FuturePosition(float seconds) {
        var tr = transform;
        Vector3 p = tr.position + tr.forward * (speedMetersPerSecond * seconds);
        return p;
    }
}
