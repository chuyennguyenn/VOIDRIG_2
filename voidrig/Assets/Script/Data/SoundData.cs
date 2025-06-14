using System;
using UnityEngine;

[Serializable]
public class SoundData : MonoBehaviour
{
    [Serializable]
    public class WeaponSound
    {
        public AudioClip shootClip;
        public AudioClip reloadClip;
    }

    [Header("Default Fallback Sounds")]
    public AudioClip defaultShootClip;
    public AudioClip defaultReloadClip;

    [Header("Weapon-Specific Sounds")]
    public WeaponSound machineGun;
    public WeaponSound shotGun;
    public WeaponSound sniper;
    public WeaponSound handGun;
    public WeaponSound smg;
    public WeaponSound burstRifle;

    public AudioClip GetShootClip(WeaponSound weaponSound)
    {
        return weaponSound != null && weaponSound.shootClip != null
            ? weaponSound.shootClip
            : defaultShootClip;
    }

    public AudioClip GetReloadClip(WeaponSound weaponSound)
    {
        return weaponSound != null && weaponSound.reloadClip != null
            ? weaponSound.reloadClip
            : defaultReloadClip;
    }
}
