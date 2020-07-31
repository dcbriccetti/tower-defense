using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speedMetersPerSecond;
    public List<Vector2> Waypoints { get; set; }
    private GameObject lastTurner;
    private int iNextWaypoint;
    public EnemyManager Manager { get; set; }

    void Update() {
        var waypoint = Waypoints[iNextWaypoint];
        var pos = transform.position;
        var wp3 = new Vector3(waypoint.x, pos.y, 9 - waypoint.y);
        var to = wp3 - pos;
        transform.LookAt(wp3);
        if (to.sqrMagnitude < 0.1)
            if (++iNextWaypoint == Waypoints.Count)
                Manager.Destroy(this);

        var toNorm = to.normalized;
        transform.Translate(toNorm * (speedMetersPerSecond * Time.deltaTime), Space.World);
    }
}