using System.Collections;
using UnityEngine;

public class Weapon: MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    private float magazine = 30;
    private float ammoCapacity = 30;
    private float reloadTime = 1f;
    private float rateOfFire = 0.05f;

    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;

    private bool isReloading = false;
    private bool isShooting = false;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireWeapon();
        }

        if (Input.GetKey(KeyCode.R) && isReloading == false) 
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
        //create bullet
        if (magazine > 0 && isShooting == false)
        {
            isShooting = true;

            StartCoroutine(Shooting(rateOfFire));
            magazine -= 1;

            Debug.Log("Bullet fired! Remaining ammo: " + magazine);
        }
        
        if (magazine <= 0)
        {
            Debug.Log("Out of Ammo!!");
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
        magazine = ammoCapacity;
        Debug.Log("Weapon reloaded!");
        isReloading = false;
    }

    private IEnumerator Shooting(float rateOfFire)
    {
        yield return new WaitForSeconds(rateOfFire);

        //spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //shoot
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        //destroy
        StartCoroutine(DestroyBullet(bullet, bulletLifeTime));

        isShooting = false;
    }
}
