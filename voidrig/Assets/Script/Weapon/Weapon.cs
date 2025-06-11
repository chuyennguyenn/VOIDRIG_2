using System.Collections;
using UnityEngine;

public class Weapon: MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    public GunData gunData;

    private int currentAmmo;
    private int magazineCapacity;
    private int totalAmmo;
    private int neededAmmo;     //use to calculate how much ammo is needed to fill the magazine

    private float reloadTime;
    private float fireRate;
    private float bulletVelocity;
    private float bulletLifeTime;

    private bool isReloading = false;
    private bool isShooting = false;

    private void Start()
    {
        //gun properties
        magazineCapacity = gunData.machineGun.magazineCapacity;
        currentAmmo = magazineCapacity; 
        totalAmmo = gunData.machineGun.totalAmmo;
        reloadTime = gunData.machineGun.reloadTime;
        fireRate = gunData.machineGun.fireRate;

        //bullet properties
        bulletVelocity = gunData.machineGun.bulletVelocity;
        bulletLifeTime = gunData.machineGun.bulletLifeTime;
        
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))   //left mouse button
        {
            FireWeapon();
        }

        if (Input.GetKey(KeyCode.R) && isReloading == false) // R key for reloading
        {
            if (currentAmmo >= magazineCapacity)
            {
                Debug.Log("Magazine is already full!");
                return;
            }

            if (totalAmmo <= 0)
            {
                Debug.Log("No ammo left to reload!");
                return;
            }

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

        if (currentAmmo > 0 && isShooting == false)
        {
            isShooting = true;

            StartCoroutine(Shooting(fireRate));
            currentAmmo -= 1;

            Debug.Log("Bullet fired! Remaining ammo: " + currentAmmo + "/" + totalAmmo);
        }
        
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of Ammo!! " + currentAmmo + "/" + totalAmmo);
        }
    }

    private IEnumerator DestroyBullet(GameObject bullet, float bulletLifetime)
    {
        yield return new WaitForSeconds(bulletLifetime);
        Destroy(bullet);
    }

    private IEnumerator ReloadWeapon(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);

        neededAmmo = magazineCapacity - currentAmmo;

        if (neededAmmo <= 0)
        {
            Debug.Log("Magazine is already full!");
            isReloading = false;
            yield break;
        }

        if (neededAmmo > totalAmmo)
        {
            neededAmmo = totalAmmo; 
        }

        totalAmmo -= neededAmmo;
        currentAmmo += neededAmmo;
        Debug.Log("Weapon reloaded!");
        isReloading = false;
    }

    private IEnumerator Shooting(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);

        //spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //shoot
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        //destroy
        StartCoroutine(DestroyBullet(bullet, bulletLifeTime));

        isShooting = false;
    }
}
