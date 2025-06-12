using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [SerializeField] private GunData gunData;

    private float currentAmmo;
    private float magazineCapacity;
    private float totalAmmo;
    private float reloadTime;
    private float fireRate;

    private float bulletVelocity;
    private float bulletLifeTime;

    private bool isReloading = false;
    private bool isShooting = false;

    private PlayerInput playerInput;
    private InputAction reloadAction;
    private System.Action<InputAction.CallbackContext> reloadCallback;

    private void Start()
    {
        // initialize gun stats from data
        magazineCapacity = gunData.machineGun.magazineCapacity;
        currentAmmo = magazineCapacity;
        totalAmmo = gunData.machineGun.totalAmmo;
        reloadTime = gunData.machineGun.reloadTime;
        fireRate = gunData.machineGun.fireRate;

        bulletVelocity = gunData.machineGun.bulletVelocity;
        bulletLifeTime = gunData.machineGun.bulletLifeTime;

        Debug.Log("Initial ammo: " + currentAmmo + "/" + totalAmmo);
    }

    private void OnEnable()
    {
        // bind reload input only after InputSystem is fully ready
        playerInput = GetComponentInParent<PlayerInput>();

        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("PlayerInput or its actions are not initialized!");
            return;
        }

        reloadAction = playerInput.actions["Reload"];
        reloadAction.Enable();

        reloadCallback = ctx => TryReload();
        reloadAction.performed += reloadCallback;
    }

    private void OnDisable()
    {
        // safely remove the reload callback
        if (reloadAction != null && reloadCallback != null)
            reloadAction.performed -= reloadCallback;
    }

    private void Update()
    {
        // fire input from mouse
        if (Mouse.current.leftButton.isPressed)
        {
            FireWeapon();
        }
    }

    private void TryReload()
    {
        Debug.Log($"Trying to reload | current: {currentAmmo}, total: {totalAmmo}");
        Debug.Log("Reload input received");

        if (!isReloading)
        {
            isReloading = true;
            StartCoroutine(ReloadWeapon(reloadTime));
        }
    }

    private void FireWeapon()
    {
        if (isReloading)
        {
            Debug.Log("Reloading, please wait...");
            return;
        }

        if (currentAmmo > 0 && !isShooting)
        {
            isShooting = true;
            StartCoroutine(Shooting(fireRate));
            currentAmmo -= 1;
            Debug.Log("Bullet fired! Remaining ammo: " + currentAmmo + "/" + totalAmmo);
        }

        if (currentAmmo <= 0 && totalAmmo <= 0)
        {
            Debug.Log("Completely out of ammo!");
        }
        else if (currentAmmo <= 0)
        {
            Debug.Log("Magazine empty! Press reload.");
        }
    }

    private IEnumerator ReloadWeapon(float reloadTime)
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);

        float missingAmmo = magazineCapacity - currentAmmo;

        if (totalAmmo <= 0)
        {
            Debug.Log("No ammo in reserve to reload!");
        }
        else if (totalAmmo < missingAmmo)
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
            Debug.Log("Partially reloaded. Ammo: " + currentAmmo + "/" + totalAmmo);
        }
        else
        {
            currentAmmo = magazineCapacity;
            totalAmmo -= missingAmmo;
            Debug.Log("Fully reloaded. Ammo: " + currentAmmo + "/" + totalAmmo);
        }

        isReloading = false;
    }

    private IEnumerator Shooting(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBullet(bullet, bulletLifeTime));

        isShooting = false;
    }

    private IEnumerator DestroyBullet(GameObject bullet, float bulletLifetime)
    {
        yield return new WaitForSeconds(bulletLifetime);
        Destroy(bullet);
    }
}
