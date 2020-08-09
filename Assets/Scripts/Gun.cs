using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
    [SerializeField] private Transform shellPrefab;
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
        flash = gunBody.Find("Barrel/Muzzle Flash").GetComponent<Light>();
        audioSource = firePoint.GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
    }

    private void Update() {
        var closest = EnemyManager.Instance.ClosestEnemyTo(transform.position, range);
        if (closest == null) return;

        Quaternion RotationToTarget() => Quaternion.LookRotation(closest.position - gunBody.position);
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
        var shell = Instantiate(shellPrefab, firePoint.position, shellRotation);
        shell.Rotate(Vector3.right, 90);
        shell.GetComponent<Rigidbody>().AddForce(gunBody.forward * firingForce);
    }

    private IEnumerator TurnOffFlash() {
        yield return new WaitForSeconds(FlashDuration);
        flash.enabled = false;
    }
}