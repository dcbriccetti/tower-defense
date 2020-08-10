using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private int damage = 100;
    [SerializeField] private float fireDelay = 0.5f;
    [SerializeField] [Range(1, 10)] private int range = 3;
    [SerializeField] private int firingForce = 800;
    [SerializeField] [Range(1, 30)] private int rotationSpeed = 15;

    private Transform gunBody;
    private Transform firePoint;
    private float nextFireTime;
    private Light flash;
    private AudioSource audioSource;
    private const float FlashDuration = 0.05f;

    private void Start() {
        nextFireTime = Time.time + Random.Range(0f, 1f);
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel/Fire Point");
        flash = firePoint.GetComponent<Light>();
        audioSource = firePoint.GetComponent<AudioSource>();
    }

    private void Update() {
        var closest = EnemyManager.Instance.ClosestEnemyTo(transform.position, range);
        if (closest == null) return;

        Quaternion RotationToTarget() {
            var aBitAhead = closest.forward * .3f;
            return Quaternion.LookRotation((closest.position + aBitAhead) - gunBody.position);
        }

        Vector3 rotation = Quaternion.Lerp(gunBody.rotation, RotationToTarget(), Time.deltaTime * rotationSpeed).eulerAngles;
        gunBody.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (!(Time.time > nextFireTime)) return;
        Fire();
        nextFireTime = Time.time + fireDelay + Random.Range(-.01f, .01f);
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