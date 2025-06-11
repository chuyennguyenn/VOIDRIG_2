using System.Collections;
using UnityEngine;

public class Weapon: MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    private float magazine = 3;
    private float ammoCapacity = 3;
    private float reloadTime = 3f;

    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;

    private bool isReloading = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
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
        if (magazine > 0)
        {

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

            //shoot
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

            //destroy
            StartCoroutine(DestroyBullet(bullet, bulletLifeTime));

            magazine -= 1;

            Debug.Log("Bullet fired! Remaining ammo: " + magazine);
        }
        
        if (magazine <= 0 && isReloading == false)
        {
            isReloading = true;
            StartCoroutine(ReloadWeapon(reloadTime));
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
}
