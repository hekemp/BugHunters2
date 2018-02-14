using System;
using UnityEngine;

/// <summary>
/// This is the base weapon.  The weapon is made up of a variety of other components to keep logic all seperated and swappable.
/// If you find yourself adding too much to this class, try creating new classes and hooking into events on this weapon
/// class instead.  
/// </summary>
[RequireComponent(typeof(WeaponAmmo))]
[RequireComponent(typeof(WeaponAudio))]
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private WeaponSettings weaponSettings;

    private WeaponAmmo weaponAmmo;
    
    private float lastFireTime;

    public event Action OnWeaponFired = () => { };
    public event Action OnDryFired = () => { };

    public uint ControllerIndex
	{
		get
		{
			PlayerHand hand = GetComponentInParent<PlayerHand>();
			if (hand.LocalHand != null)
			{
				return hand.LocalHand.controller.index;
			}
			return 0;
		}
	}

    private void Awake()
    {
        GetWeaponComponents();
        
        GetComponent<WeaponInput>().OnTriggerPulled += TryFire;

        weaponAmmo.Reload();
    }

    private void GetWeaponComponents()
    {
		weaponAmmo = transform.GetComponent<WeaponAmmo>();
    }

    private void TryFire()
    {
        if (CanFire())
        {
            Fire();
        }
        else
        {
            OnDryFired();
        }
    }

    private bool CanFire()
    {
        return weaponAmmo.HasAmmoInClip() &&
            weaponAmmo.IsReloading == false &&
            HasMetFireDelayRequirement();
    }

    private bool HasMetFireDelayRequirement()
    {
        return Time.time - lastFireTime > weaponSettings.FireDelay;
    }

    private void Fire()
    {
        weaponAmmo.TakeAmmo();
        OnWeaponFired();
        lastFireTime = Time.time;
    }
}