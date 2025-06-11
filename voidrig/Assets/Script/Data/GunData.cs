using System;
using UnityEngine;


public class GunData: MonoBehaviour
{
    [Serializable]
    public class Attribute
    {
        public int magazineCapacity;
        public int totalAmmo;
        public int damage;
        public float bulletVelocity;
        public float knockBack;
        public float bulletLifeTime;
        public float accuracy;
        public float fireRate;
        public bool scatter;
    }

    Attribute machineGun = new Attribute
    {
        magazineCapacity = 30,
        totalAmmo = 120,
        damage = 3,
        bulletVelocity = 180f,
        knockBack = 2f,
        bulletLifeTime = 2f,
        accuracy = 0.7f,
        fireRate = 0.05f,
        scatter = false
    };

    Attribute shotGun = new Attribute
    {
        magazineCapacity = 7,
        totalAmmo = 28,
        damage = 1,
        bulletVelocity = 180f,
        knockBack = 10f,
        bulletLifeTime = 0.5f,
        accuracy = 0.3f,
        fireRate = 0.5f,
        scatter = true
    };

    Attribute sniper = new Attribute
    {
        magazineCapacity = 5,
        totalAmmo = 20,
        damage = 10,
        bulletVelocity = 540f,
        knockBack = 1f,
        bulletLifeTime = 3f,
        accuracy = 1f,
        fireRate = 1f,
        scatter = false
    };

    Attribute handGun = new Attribute
    {
        magazineCapacity = 10,
        totalAmmo = 40,
        damage = 4,
        bulletVelocity = 180f,
        knockBack = 4f,
        bulletLifeTime = 2f,
        accuracy = 0.85f,
        fireRate = 0.1f,
        scatter = false
    };
}
