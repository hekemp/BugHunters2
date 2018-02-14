using System;
using UnityEngine;

/// <summary>
/// This is a class to hold all non-ammo related weapon settings.
/// If you want to add more options/settings to the weapon, this is a good default place.
/// Make sure to add a new class if it gets complicated or needs real logic though (ex. the WeaponAmmo class)
/// </summary>
[Serializable]
public class WeaponSettings
{
    [SerializeField]
    [Tooltip("Minimum time between trigger pulls")]
    private float fireDelay = 0.1f;

    public float FireDelay { get { return fireDelay; } }
}