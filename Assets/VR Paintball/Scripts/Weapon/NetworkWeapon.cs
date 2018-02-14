using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkWeapon : NetworkBehaviour
{
    internal void LaunchProjectile(Vector3 position, Quaternion rotation)
    {
        CmdLaunchProjectile(position, rotation);
    }

    [Command]
    private void CmdLaunchProjectile(Vector3 position, Quaternion rotation)
    {
        GetComponentInChildren<WeaponProjectileLauncher>().SpawnProjectile(position, rotation);
    }
}