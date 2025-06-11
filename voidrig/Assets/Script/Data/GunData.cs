using System;
using UnityEngine;

[Serializable]
public class GunData : MonoBehaviour
{
    [Serializable]
    public class Attribute
    {
        public int magazineCapacity;
        public int totalAmmo;

        public float damage;
        public float bulletVelocity;
        public float knockBack;
        public float bulletLifeTime;
        public float accuracy;
        public float fireRate;
        public float reloadTime;

        public bool scatter;
    }

    public Attribute machineGun = new Attribute
    {
        magazineCapacity = 30,
        totalAmmo = 120,
        damage = 3f,
        bulletVelocity = 180f,
        knockBack = 2f,
        bulletLifeTime = 2f,
        accuracy = 0.7f,
        fireRate = 0.05f,
        reloadTime = 1f,
        scatter = false
    };

    public Attribute shotGun = new Attribute
    {
        magazineCapacity = 7,
        totalAmmo = 28,
        damage = 1f,
        bulletVelocity = 180f,
        knockBack = 10f,
        bulletLifeTime = 0.5f,
        accuracy = 0.3f,
        fireRate = 0.5f,
        reloadTime = 1f,
        scatter = true
    };

    public Attribute sniper = new Attribute
    {
        magazineCapacity = 5,
        totalAmmo = 20,
        damage = 10f,
        bulletVelocity = 540f,
        knockBack = 1f,
        bulletLifeTime = 3f,
        accuracy = 1f,
        fireRate = 1f,
        reloadTime = 2f,
        scatter = false
    };

    public Attribute handGun = new Attribute
    {
        magazineCapacity = 10,
        totalAmmo = 40,
        damage = 4f,
        bulletVelocity = 180f,
        knockBack = 4f,
        bulletLifeTime = 2f,
        accuracy = 0.85f,
        fireRate = 0.1f,
        reloadTime = 1f,
        scatter = false
    };
}
