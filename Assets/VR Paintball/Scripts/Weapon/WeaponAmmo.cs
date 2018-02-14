using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class handles all the weapon's ammo data and logic.
/// It controls reloading and tells the weapon if there's ammo available or not.
/// </summary>
[RequireComponent(typeof(Weapon))]
public class WeaponAmmo : MonoBehaviour
{
    private int remainingAmmoOutOfClip;
    private int remainingAmmoInClip;
    [SerializeField]
    [Tooltip("The amount of ammo the weapon has when it's instantiated")]
    private int startingAmmo = 18;
    [SerializeField]
    [Tooltip("The amount of ammo the weapon can fire before needing to be reloaded")]
    private int clipSize = 6;
    [SerializeField]
    [Tooltip("How long it takes for the weapon to reload. While reloading it can't be fired. (this should match the animation time)")]
    private float reloadTime = 1f;

    public event Action OnReloadStarted = () => { };
    public event Action OnReloadCompleted = () => { };
    public event Action<WeaponAmmoData> OnAmmoChanged = (weaponAmmoData) => { };
    public bool IsReloading { get; private set; }

    private void Awake()
    {
        remainingAmmoOutOfClip = startingAmmo;
    }

    public bool HasAmmoInClip()
    {
        return remainingAmmoInClip > 0;
    }

    public void TakeAmmo()
    {
        remainingAmmoInClip--;
        SendAmmoChanged();
    }

    private void SendAmmoChanged()
    {
        OnAmmoChanged(new WeaponAmmoData(remainingAmmoInClip,
        remainingAmmoOutOfClip));
    }

    public void Reload()
    {
        if (IsReloading == false)
        {
            StartCoroutine(ReloadAsync());
        }
    }

    private IEnumerator ReloadAsync()
    {
        int desiredAmmoToMove = clipSize - remainingAmmoInClip;
        desiredAmmoToMove = Mathf.Max(desiredAmmoToMove, 0);
        int ammoToMove = Math.Min(desiredAmmoToMove, remainingAmmoOutOfClip);
        if (ammoToMove > 0)
        {
            IsReloading = true;
            OnReloadStarted();
            yield return new WaitForSeconds(reloadTime);
            remainingAmmoInClip += ammoToMove;
            remainingAmmoOutOfClip -= ammoToMove;
            IsReloading = false;
            OnReloadCompleted();
            SendAmmoChanged();
        }
    }
}