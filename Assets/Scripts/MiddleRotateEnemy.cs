    using UnityEngine;

    public class MiddleRotateEnemy : Enemy {
        private Transform middle;

        private new void Start() {
            base.Start();
            middle = transform.Find("Middle");
        }
        private new void FixedUpdate() {
            base.FixedUpdate();
            if (middle) {
                middle.Rotate(Vector3.up, 90 * Time.deltaTime);
            }
        }
    }
