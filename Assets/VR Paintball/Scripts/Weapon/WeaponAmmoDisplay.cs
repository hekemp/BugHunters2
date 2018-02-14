using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To use this class, attach it to a text object that's a child of the weapon.
/// The text values will automatically update as ammo changes.
/// </summary>
public class WeaponAmmoDisplay : MonoBehaviour
{
    private void Awake()
    {
        GetComponentInParent<WeaponAmmo>().OnAmmoChanged += HandleAmmoChanged;
    }

    private void HandleAmmoChanged(WeaponAmmoData weaponAmmoData)
    {
        GetComponent<Text>().text =
            string.Format("{0} / {1}", weaponAmmoData.RemainingInClip, weaponAmmoData.RemainingOutOfClip);
    }
}