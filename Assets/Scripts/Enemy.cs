using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] [Range(10, 1000)] private int health = 100;
    [SerializeField] public int killValue = 5;
    [SerializeField] [Range(0, 20)] private float speedMetersPerSecond = 1;
    [SerializeField] [Range(1, 30)] public int rotationSpeed = 8;
    public List<Vector2> Waypoints { get; set; }
    private int iNextWaypoint;
    private readonly EnemyManager enemyManager = EnemyManager.instance;
    private WaypointTraveller wpt;
    private bool alive = true;

    protected void Start() {
        wpt = new WaypointTraveller(Waypoints);
        var w = Waypoints[0];
        transform.LookAt(new Vector3(w.x, transform.position.y, w.y));
    }

    public void FixedUpdate() {
        if (transform.position.y < -10) { // Sometimes they fall off
            enemyManager.Destroy(this, false);
            return;
        }

        var pos = transform.position;
        var adjustedWaypoint = wpt.GetAdjustedWaypoint(iNextWaypoint, pos.y);
        Quaternion rotTo = Quaternion.LookRotation(adjustedWaypoint - pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotTo, rotationSpeed * Time.deltaTime);
        var toWaypoint = adjustedWaypoint - pos;
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
            var head = transform.Find("HeadPosition/Head");
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
        var distanceToTravel = speedMetersPerSecond * seconds;
        var pos = wpt.PositionThroughWaypoints(transform.position, iNextWaypoint, distanceToTravel);
        return new Vector3(pos.x, 0.2f, pos.z); // Don’t aim at my “feet”
    }
}
