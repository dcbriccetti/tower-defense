    using System.Collections.Generic;
    using UnityEngine;

    public class AbstractEnemy : MonoBehaviour {
        public float speedMetersPerSecond;
        public List<Vector2> Waypoints { get; set; }
        private int iNextWaypoint;
        private EnemyManager enemyManager = EnemyManager.Instance;

        protected void Update() {
            if (transform.position.y < -10) { // Sometimes they fall off
                enemyManager.Destroy(this, false);
                return;
            }

            var waypoint = Waypoints[iNextWaypoint];
            var pos = transform.position;
            var waypoint3 = new Vector3(waypoint.x, pos.y, waypoint.y);
            transform.LookAt(waypoint3);
            var toWaypoint = waypoint3 - pos;
            if (toWaypoint.sqrMagnitude < 0.1)
                if (++iNextWaypoint == Waypoints.Count)
                    enemyManager.Destroy(this, true);

            transform.Translate(toWaypoint.normalized * (speedMetersPerSecond * Time.deltaTime), Space.World);
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Projectile")) {
                enemyManager.Destroy(this, false);
            }
        }
    }