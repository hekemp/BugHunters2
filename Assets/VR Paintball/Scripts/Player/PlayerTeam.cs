using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("The materials the player will use for the body. (index 0 = team 1)")]
    private Material[] bodyMaterials;

    [SerializeField]
    [Tooltip("The materials the player will use for the gun. (index 0 = team 1)")]
    private Material[] gunMaterials;

    public Action OnTeamChanged = delegate { };

    [SyncVar]
    public int Team;
    
    public PlayerTeam Local { get; private set; }

    public override void OnStartLocalPlayer()
    {
        Local = this;
        base.OnStartLocalPlayer();

        CmdRequestTeam();
    }

    [Command]
    private void CmdRequestTeam()
    {
        TeamController.Instance.RequestTeam(this);
    }

    public void SetTeam(int team)
    {
        Team = team;
        RpcSetTeam(team);
    }

    [ClientRpc]
    public void RpcSetTeam(int team)
    {
        Team = team;
        SetTeamMaterials();
        gameObject.name = "Player - Team " + team;

        OnTeamChanged();
    }

    private void SetTeamMaterials()
    {
        GetComponentInChildren<PlayerBody>().GetComponent<Renderer>().material = GetBodyMaterial();

        GetComponentInChildren<Weapon>().GetComponentInChildren<Renderer>().material = GetWeaponMaterial();
    }

    private Material GetWeaponMaterial()
    {
        if (gunMaterials.Length < Team)
        {
            throw new Exception("No Weapon material set for team " + Team + ".  Array size is " + gunMaterials.Length);
        }

        return gunMaterials[Team - 1];
    }

    private Material GetBodyMaterial()
    {
        if (bodyMaterials.Length < Team)
        {
            throw new Exception("No Weapon material set for team " + Team + ".  Array size is " + bodyMaterials.Length);
        }

        return bodyMaterials[Team - 1];
    }
}