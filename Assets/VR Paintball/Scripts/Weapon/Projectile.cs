using System;
using UnityEngine;
using UnityEngine.Networking;

internal class Projectile : NetworkBehaviour
{
    public int SpawnedByTeam { get; set; }
    public NetworkPlayer SpawnedByPlayer { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        NetworkPlayer player = collision.gameObject.GetComponentInParent<NetworkPlayer>();

        if (player != null)
        {
            if (GameSettings.Instance.AllowTeamKills == false && player.GetComponent<PlayerTeam>().Team == SpawnedByTeam)
            {
                Debug.Log("Team damage not allowed.");
            }
            else if (isServer)
            {
                HandlePlayerHit(player);
            }
        }
        else
        {
            HandleEnvironmentHit(collision);
        }
        Destroy(this.gameObject);
    }

    private void HandlePlayerHit(NetworkPlayer player)
    {
        player.RpcTakeHit(SpawnedByPlayer.GetComponent<NetworkIdentity>());
    }

    private void HandleEnvironmentHit(Collision collision)
    {
        var collisionPoint = collision.contacts[0].point;
        var normal = collision.contacts[0].normal;
        Ray ray = new Ray(collisionPoint + normal * 0.25f, normal * -1f);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 0.25f))
        {
            ShowDecal(hitInfo.point, normal);
        }
    }

    private void ShowDecal(Vector3 point, Vector3 normal)
    {
        DecalController.Instance.SpawnDecal(point, normal);
    }

    [Command]
    private void CmdOnHitSomething(Vector3 point, Vector3 normal)
    {
        
    }
}