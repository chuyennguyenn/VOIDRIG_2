using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Data")]
    [SerializeField] private GunData gunData;
    [SerializeField] private SoundData soundData;

    private GunData.Attribute activeGun;
    private SoundData.WeaponSound activeSound;

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

<<<<<<< Updated upstream
    private void Awake()
    {
        readyToShoot = true;
=======
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        readyToShoot = true;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
>>>>>>> Stashed changes
    }

    private void Start()
    {
        switch (gameObject.tag)
        {
            case "MachineGun": SetActiveGun(gunData.machineGun, soundData.machineGun); break;
            case "ShotGun": SetActiveGun(gunData.shotGun, soundData.shotGun); break;
            case "Sniper": SetActiveGun(gunData.sniper, soundData.sniper); break;
            case "HandGun": SetActiveGun(gunData.handGun, soundData.handGun); break;
            case "SMG": SetActiveGun(gunData.smg, soundData.smg); break;
            case "BurstRifle": SetActiveGun(gunData.burstRifle, soundData.burstRifle); break;
            default:
                Debug.LogWarning("Unknown tag, defaulting to MachineGun");
                SetActiveGun(gunData.machineGun, soundData.machineGun);
                break;
        }
    }

    private void OnEnable()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput?.actions == null)
        {
            Debug.LogError("PlayerInput or actions not assigned");
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
        if (reloadAction != null) reloadAction.performed -= ctx => TryReload();
        if (switchModeAction != null) switchModeAction.performed -= ctx => SwitchMode();
    }

    private void Update()
    {
        isShooting = currentShootingMode == GunData.ShootingMode.Auto
            ? attackAction.IsPressed()
            : attackAction.WasPressedThisFrame();

        if (readyToShoot && isShooting)
        {
<<<<<<< Updated upstream
            burstBulletsLeft = bulletsPerBurst;
            StartCoroutine(ShootRepeatedly());
=======
            if (currentAmmo > 0)
            {
                animator.SetTrigger("Recoil");
                SoundManager.Instance?.PlayShoot();
                burstBulletsLeft = bulletsPerBurst;
                StartCoroutine(ShootRepeatedly());
            }
            else
            {
                Debug.Log("No ammo to shoot");
            }
>>>>>>> Stashed changes
        }
    }

    private IEnumerator ShootRepeatedly()
    {
        readyToShoot = false;

        while (burstBulletsLeft > 0 && currentAmmo > 0 && !isReloading)
        {
<<<<<<< Updated upstream
            FireBullet();
            burstBulletsLeft--;
=======
            if (gameObject.CompareTag("ShotGun"))
            {
                for (int i = 0; i < bulletsPerBurst; i++)
                    FireBullet();
                burstBulletsLeft = 0;
            }
            else
            {
                FireBullet();
                burstBulletsLeft--;
            }
>>>>>>> Stashed changes

            if (currentShootingMode == GunData.ShootingMode.Single)
                break;

            yield return new WaitForSeconds(burstFireInterval);
        }

<<<<<<< Updated upstream
=======
        animator.SetTrigger("RecoilRecover");
>>>>>>> Stashed changes
        ResetShot();
    }

    private void FireBullet()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Tried to fire with 0 ammo");
            return;
        }

<<<<<<< Updated upstream
        // Calculate direction with spread
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate and shoot bullet
=======
        muzzleEffect?.GetComponent<ParticleSystem>()?.Play();

        Vector3 direction = CalculateDynamicSpread().normalized;
>>>>>>> Stashed changes
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = direction;
        bullet.GetComponent<Rigidbody>().AddForce(direction * bulletVelocity, ForceMode.Impulse);
        Destroy(bullet, bulletLifeTime);

        currentAmmo--;
        Debug.Log($"Fired bullet. Ammo left: {currentAmmo}/{totalAmmo}");
    }

    private void TryReload()
    {
        if (!isReloading)
        {
            isReloading = true;
<<<<<<< Updated upstream
            Debug.Log("Reloading...");
=======
            animator.SetTrigger("Reload");
            SoundManager.Instance?.PlayReload();
>>>>>>> Stashed changes
            StartCoroutine(ReloadWeapon(reloadTime));
        }
    }

    private IEnumerator ReloadWeapon(float time)
    {
        yield return new WaitForSeconds(time);

        int missing = magazineCapacity - currentAmmo;

        if (totalAmmo <= 0)
        {
            Debug.Log("No reserve ammo");
        }
        else if (totalAmmo < missing)
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            currentAmmo = magazineCapacity;
            totalAmmo -= missing;
        }

<<<<<<< Updated upstream
=======
        animator.SetTrigger("ReloadRecover");
>>>>>>> Stashed changes
        isReloading = false;
        Debug.Log($"Reloaded. Current ammo: {currentAmmo}/{totalAmmo}");
    }

    private void SetActiveGun(GunData.Attribute gun, SoundData.WeaponSound sound)
    {
        activeGun = gun;
        activeSound = sound;

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
    }

    public void SwitchMode()
    {
        if (availableModes.Length > 1)
        {
            currentModeIndex = (currentModeIndex + 1) % availableModes.Length;
            currentShootingMode = availableModes[currentModeIndex];
            Debug.Log("Switched firing mode to: " + currentShootingMode);
        }
    }

    private Vector3 CalculateDynamicSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 target = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(100);
        Vector3 direction = target - bulletSpawn.position;

        float distance = direction.magnitude;
        float adjustedSpread = spreadIntensity * (distance / 10f);

        float x = Random.Range(-adjustedSpread, adjustedSpread);
        float y = Random.Range(-adjustedSpread, adjustedSpread);

        return direction + new Vector3(x, y, 0);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
}
