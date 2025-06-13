using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    // === Inspector-Assigned References ===
    [Header("References")]
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    // === Gun Configuration Data ===
    [Header("Gun Data")]
    [SerializeField] private GunData gunData;

    // === Runtime Gun Attributes ===
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

    // === Shooting Mode Tracking ===
    private GunData.ShootingMode[] availableModes;
    private int currentModeIndex;
    private GunData.ShootingMode currentShootingMode;
    private int burstBulletsLeft;

    // === Internal State Flags ===
    private bool isShooting = false;
    private bool readyToShoot = true;
    private bool allowReset = true;
    private bool isReloading = false;

    // === Input System Actions ===
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
        // Fallback to main camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            Debug.LogWarning("PlayerCamera not assigned. Defaulting to Camera.main.");
        }

        // Equip the default gun
        SetActiveGun(gunData.machineGun);
    }

    private void OnEnable()
    {
        // Initialize input system
        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("PlayerInput or its actions are not initialized!");
            return;
        }

        // Bind actions
        attackAction = playerInput.actions["Attack"];
        reloadAction = playerInput.actions["Reload"];
        switchModeAction = playerInput.actions["SwitchMode"];

        // Enable inputs
        attackAction.Enable();
        reloadAction.Enable();
        switchModeAction.Enable();

        // Hook up callbacks
        reloadAction.performed += ctx => TryReload();
        switchModeAction.performed += ctx => SwitchMode();
    }

    private void OnDisable()
    {
        // Unhook callbacks to prevent memory leaks
        if (reloadAction != null)
            reloadAction.performed -= ctx => TryReload();

        if (switchModeAction != null)
            switchModeAction.performed -= ctx => SwitchMode();
    }

    private void Update()
    {
        // Determine firing input based on mode
        isShooting = currentShootingMode == GunData.ShootingMode.Auto
            ? attackAction.IsPressed()
            : attackAction.WasPressedThisFrame();

        // Start firing routine
        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            StartCoroutine(ShootRepeatedly());
        }
    }

    private IEnumerator ShootRepeatedly()
    {
        readyToShoot = false;

        // Loop for burst/auto shooting
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
        // Exit if out of ammo
        if (currentAmmo <= 0)
        {
            Debug.Log(totalAmmo <= 0 ? "Completely out of ammo!" : "Magazine empty! Press reload.");
            return;
        }

        // Calculate direction with spread
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate and shoot bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        Destroy(bullet, bulletLifeTime);

        // Update ammo
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
        // Cache stats from GunData
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
        // Determine target point via raycast
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(100);

        // Add randomized spread
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }
}
