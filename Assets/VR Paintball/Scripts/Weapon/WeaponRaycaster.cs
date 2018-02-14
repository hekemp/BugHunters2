using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script shows how to fire a weapon with a raycast instead of a projectile.
/// If you want to use an instant shot weapon like a faster gun where projectiles don't make sense, 
/// this script is a good starting point.  It requires a shootable component to be added to 
/// whatever's being shot, and will trigger the TakeShot method on the shootable.
/// </summary>
public class WeaponRaycaster : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The transform that determines where the raycast starts and the direction it fires in")]
    private Transform fireRaycastPoint;

    [SerializeField]
    [Tooltip("Max distance for the shot to hit")]
    private float fireRange = 100f;

    private void Start()
    {
        GetComponent<Weapon>().OnWeaponFired += SendWeaponFired;
    }

    private void SendWeaponFired()
    {
        FireWeaponRaycast(fireRaycastPoint.position, fireRaycastPoint.forward);
    }

    private void FireWeaponRaycast(Vector3 position, Vector3 forward)
    {
        Ray ray = new Ray(fireRaycastPoint.position, fireRaycastPoint.forward);
        Debug.DrawRay(fireRaycastPoint.position, fireRaycastPoint.forward * fireRange, Color.red, 2f);
        var hitInfos = Physics.RaycastAll(ray, fireRange).OrderBy(t => t.distance);

        foreach (var hit in hitInfos)
        {
            Shootable shootable = hit.collider.GetComponent<Shootable>();
            if (shootable != null)
            {
                shootable.TakeShot(hit);
                Debug.DrawRay(hit.point, hit.normal, Color.green, 10f);
                break;
            }
        }
    }
}