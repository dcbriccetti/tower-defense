using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform shellPrefab;
    public float fireDelay = 0.5f;
    public int range = 3;
    public int firingForce = 800;
    public int rotationSpeed = 10;

    private Transform gunBody;
    private Transform firePoint;
    private float nextFireTime;
    private Light flash;
    private const float FlashDuration = 0.05f;

    private void Start() {
        nextFireTime = Time.time + Random.Range(0f, 1f);
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel/Fire Point");
        flash = gunBody.Find("Barrel/Muzzle Flash").GetComponent<Light>();
    }

    private void Update() {
        var closest = EnemyManager.Instance.ClosestEnemyTo(transform.position, range);
        if (closest != null) {
            Vector3 directionToTarget = closest.position - gunBody.position;
            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
            Vector3 rotation = Quaternion.Lerp(gunBody.rotation, rotationToTarget, Time.deltaTime * rotationSpeed).eulerAngles;
            gunBody.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            FireWhenReady();
        }
    }

    private void FireWhenReady() {
        if (Time.time < nextFireTime) return;
        firePoint.GetComponent<AudioSource>().Play();
        flash.enabled = true;
        StartCoroutine(nameof(TurnOffFlash));
        var shellRotation = gunBody.rotation;
        var shell = Instantiate(shellPrefab, firePoint.position, shellRotation);
        shell.Rotate(Vector3.right, 90);
        shell.GetComponent<Rigidbody>().AddForce(gunBody.forward * firingForce);
        nextFireTime = Time.time + fireDelay;
    }

    private IEnumerator TurnOffFlash() {
        yield return new WaitForSeconds(FlashDuration);
        flash.enabled = false;
    }
}