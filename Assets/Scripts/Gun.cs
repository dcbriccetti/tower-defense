using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour {
    public new string name;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private int damage = 100;
    [SerializeField] private float fireDelay = 0.5f;
    [Tooltip("The range within which the gun will track and fire")]
    [SerializeField] [Range(1, 10)] private int range = 3;
    [SerializeField] private int firingForce = 800;
    [SerializeField] [Range(1, 30)] private int rotationSpeed = 15;
    public EnemyManager.TargetInfo? selectedTarget;
    [SerializeField] private float nextFireTime;

    private Transform gunBody;
    private Transform firePoint;
    private Light flash;
    private AudioSource audioSource;
    private readonly EnemyManager enemyManager = EnemyManager.instance;
    private const float FlashDuration = 0.05f;

    private void Start() {
        nextFireTime = Time.time + Random.Range(0f, 1f);
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel/Fire Point");
        flash = firePoint.GetComponent<Light>();
        audioSource = firePoint.GetComponent<AudioSource>();
    }

    private void Update() {
        if (selectedTarget.HasValue && enemyManager.IsAlive(selectedTarget.Value.enemy)) {
            var aim = selectedTarget.Value.futurePos - gunBody.position;
            var interpolatedRotation = Quaternion.Lerp(gunBody.rotation, Quaternion.LookRotation(aim),
                Time.deltaTime * rotationSpeed);
            gunBody.rotation = interpolatedRotation;
            var angleToAimPoint = Quaternion.Angle(Quaternion.LookRotation(aim), interpolatedRotation);

            if (Math.Abs(angleToAimPoint) < 2 && Time.time > nextFireTime) {
                Debug.DrawLine(firePoint.position, selectedTarget.Value.futurePos, Color.red, 0.2f);
                Fire();
                nextFireTime = Time.time + fireDelay + Random.Range(0, .01f);
                selectedTarget = null;
            }
        } else {
            float secondsUntilFire = Math.Max(0, nextFireTime - Time.time);
            var maybeBestTargetInfo = enemyManager.BestEnemyTarget(transform, range, secondsUntilFire);
            if (! maybeBestTargetInfo.HasValue) return;
            selectedTarget = maybeBestTargetInfo.Value;
            Debug.DrawLine(firePoint.position, selectedTarget.Value.futurePos, Color.yellow, secondsUntilFire);
        }
    }

    private void Fire() {
        audioSource.Play();
        flash.enabled = true;
        StartCoroutine(nameof(TurnOffFlash));
        var shellRotation = gunBody.rotation;
        var shellTransform = Instantiate(shellPrefab, firePoint.position, shellRotation);
        var shell = shellTransform.GetComponent<Shell>();
        shell.damage = damage;
        shellTransform.Rotate(Vector3.right, 90);
        shellTransform.GetComponent<Rigidbody>().AddForce(gunBody.forward * firingForce);
    }

    private IEnumerator TurnOffFlash() {
        yield return new WaitForSeconds(FlashDuration);
        flash.enabled = false;
    }
}