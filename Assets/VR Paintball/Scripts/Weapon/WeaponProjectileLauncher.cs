using UnityEngine;
using UnityEngine.Networking;

public class WeaponProjectileLauncher : MonoBehaviour
{
    [SerializeField]
    private Transform muzzlePointTransform;

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private float launchSpeed = 500f;

    private void Start()
    {
        GetComponent<Weapon>().OnWeaponFired += WeaponProjectileLauncher_OnWeaponFired;
    }

    private void WeaponProjectileLauncher_OnWeaponFired()
    {
        GetComponentInParent<NetworkWeapon>().LaunchProjectile(muzzlePointTransform.position, muzzlePointTransform.rotation);
    }

    public void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        Projectile projectile = Instantiate(projectilePrefab, position, rotation) as Projectile;
        projectile.GetComponent<Rigidbody>().velocity = muzzlePointTransform.forward * launchSpeed;
        projectile.SpawnedByTeam = GetComponentInParent<PlayerTeam>().Team;
        projectile.SpawnedByPlayer = GetComponentInParent<NetworkPlayer>();

        NetworkServer.Spawn(projectile.gameObject);
    }
}