using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform shellPrefab;
    public Transform muzzleFlashPrefab;
    public float fireDelay = 0.5f;
    public int range = 3;
    public EnemyManager EnemyManager { get; set; }

    private Transform gunBody;
    private Transform firePoint;
    private Transform flashPoint;
    private float nextFireTime;
    private Light flash;
    private Transform muzzleFlash;
    private const float FlashDuration = 0.12f;

    void Start() {
        nextFireTime = Time.time + Random.Range(0f, 1f);
        gunBody = transform.Find("Body");
        firePoint = gunBody.Find("Barrel/Fire Point");
        flashPoint = gunBody.Find("Barrel/Flash Point");
        muzzleFlash = Instantiate(muzzleFlashPrefab);
        flash = muzzleFlash.GetComponent<Light>();
    }

    void Update() {
        if (!EnemyManager) return;
        var closest = EnemyManager.ClosestEnemyTo(transform.position, range);
        if (closest != null) {
            Vector3 aimAt = closest.position + closest.forward * .1f; // A little bit ahead
            gunBody.LookAt(aimAt);
            FireWhenReady();
        } else {
            gunBody.rotation = Quaternion.identity;
        }
    }

    private void FireWhenReady() {
        if (Time.time < nextFireTime) return;
        muzzleFlash.position = flashPoint.position;
        flash.enabled = true;
        StartCoroutine("TurnOffFlash");
        var shellRotation = gunBody.rotation;
        var shell = Instantiate(shellPrefab, firePoint.position, shellRotation);
        shell.Rotate(Vector3.right, 90);
        shell.GetComponent<Rigidbody>().AddForce(gunBody.forward * 1000f);
        nextFireTime = Time.time + fireDelay + Random.Range(0, .1f);
    }

    IEnumerator TurnOffFlash() {
        yield return new WaitForSeconds(FlashDuration);
        flash.enabled = false;
    }
}