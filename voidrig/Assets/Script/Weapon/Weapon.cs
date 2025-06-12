using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Gun Data")]
    [SerializeField] private GunData gunData;

    private GunData.Attribute activeGun;
    private int currentAmmo;
    private int magazineCapacity;
    private int totalAmmo;
    private float reloadTime;
    private float fireRate;
    private float bulletVelocity;
    private float bulletLifeTime;
    private float spreadIntensity;
    private int bulletsPerBurst;
    private float burstFireInterval;

    private GunData.ShootingMode[] availableModes;
    private int currentModeIndex;
    private GunData.ShootingMode currentShootingMode;
    private int burstBulletsLeft;

    private bool isShooting = false;
    private bool readyToShoot = true;
    private bool allowReset = true;
    private bool isReloading = false;

    private PlayerInput playerInput;
    private InputAction attackAction;
    private InputAction reloadAction;
    private InputAction switchModeAction;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            Debug.LogWarning("PlayerCamera not assigned. Defaulting to Camera.main.");
        }

        SetActiveGun(gunData.machineGun);
    }

    private void OnEnable()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("PlayerInput or its actions are not initialized!");
            return;
        }

        attackAction = playerInput.actions["Attack"];
        reloadAction = playerInput.actions["Reload"];
        switchModeAction = playerInput.actions["SwitchMode"];

        attackAction.Enable();
        reloadAction.Enable();
        switchModeAction.Enable();

        reloadAction.performed += ctx => TryReload();
        switchModeAction.performed += ctx => SwitchMode();
    }

    private void OnDisable()
    {
        if (reloadAction != null)
            reloadAction.performed -= ctx => TryReload();

        if (switchModeAction != null)
            switchModeAction.performed -= ctx => SwitchMode();
    }

    private void Update()
    {
        isShooting = currentShootingMode == GunData.ShootingMode.Auto
            ? attackAction.IsPressed()
            : attackAction.WasPressedThisFrame();

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            StartCoroutine(ShootRepeatedly());
        }
    }

    private IEnumerator ShootRepeatedly()
    {
        readyToShoot = false;

        while (burstBulletsLeft > 0 && currentAmmo > 0 && !isReloading)
        {
            FireBullet();
            burstBulletsLeft--;

            if (currentShootingMode == GunData.ShootingMode.Single)
                break;

            yield return new WaitForSeconds(burstFireInterval);
        }

        ResetShot();
    }

    private void FireBullet()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log(totalAmmo <= 0 ? "Completely out of ammo!" : "Magazine empty! Press reload.");
            return;
        }

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        Destroy(bullet, bulletLifeTime);

        currentAmmo--;
        Debug.Log("Bullet fired! Remaining ammo: " + currentAmmo + "/" + totalAmmo);
    }

    private void TryReload()
    {
        if (!isReloading)
        {
            isReloading = true;
            Debug.Log("Reloading...");
            StartCoroutine(ReloadWeapon(reloadTime));
        }
    }

    private IEnumerator ReloadWeapon(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);

        int missingAmmo = magazineCapacity - currentAmmo;

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

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private void SetActiveGun(GunData.Attribute gun)
    {
        activeGun = gun;

        magazineCapacity = gun.magazineCapacity;
        currentAmmo = magazineCapacity;
        totalAmmo = gun.totalAmmo;
        reloadTime = gun.reloadTime;
        fireRate = gun.fireRate;
        bulletVelocity = gun.bulletVelocity;
        bulletLifeTime = gun.bulletLifeTime;
        spreadIntensity = gun.spreadIntensity;
        bulletsPerBurst = gun.bulletsPerBurst;
        burstFireInterval = gun.burstFireInterval;

        availableModes = gun.availableModes != null && gun.availableModes.Length > 0
            ? gun.availableModes
            : new GunData.ShootingMode[] { gun.shootingMode };

        currentModeIndex = 0;
        currentShootingMode = availableModes[currentModeIndex];
        burstBulletsLeft = bulletsPerBurst;

        Debug.Log($"Equipped weapon: {gun} | Mode: {currentShootingMode} | Ammo: {currentAmmo}/{totalAmmo}");
    }

    public void SwitchMode()
    {
        if (availableModes.Length > 1)
        {
            currentModeIndex = (currentModeIndex + 1) % availableModes.Length;
            currentShootingMode = availableModes[currentModeIndex];
            Debug.Log("Switched shooting mode to: " + currentShootingMode);
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(100);

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }
}
