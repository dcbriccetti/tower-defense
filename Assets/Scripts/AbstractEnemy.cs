    using System.Collections.Generic;
    using UnityEngine;

    public class AbstractEnemy : MonoBehaviour {
        public float speedMetersPerSecond;
        public List<Vector2> Waypoints { get; set; }
        private int iNextWaypoint;

        protected void Update() {
            var waypoint = Waypoints[iNextWaypoint];
            var pos = transform.position;
            var waypoint3 = new Vector3(waypoint.x, pos.y, waypoint.y);
            transform.LookAt(waypoint3);
            var toWaypoint = waypoint3 - pos;
            if (toWaypoint.sqrMagnitude < 0.1)
                if (++iNextWaypoint == Waypoints.Count)
                    EnemyManager.Instance.Destroy(this);

            transform.Translate(toWaypoint.normalized * (speedMetersPerSecond * Time.deltaTime), Space.World);
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Projectile")) {
                EnemyManager.Instance.Destroy(this);
            }
        }
    }