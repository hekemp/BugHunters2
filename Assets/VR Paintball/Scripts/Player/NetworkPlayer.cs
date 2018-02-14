using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(PlayerTeam))]
public partial class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private PlayerHand networkLeftHand;
    [SerializeField]
    private PlayerHand networkRightHand;
    [SerializeField]
    private PlayerHead networkHead;

    public static NetworkPlayer Local { get; private set; }
    public static Transform LocalPlayerRootTransform { get; private set; }

	private bool controllersLinked;
    private PlayerTeam playerTeam;
	private string playerName;

	public bool IsOnLocalPlayersTeam { get { return playerTeam.Team == Local.playerTeam.Team; } }

    private void Awake()
    {
        playerTeam = GetComponent<PlayerTeam>();
        playerTeam.OnTeamChanged += () => Respawn();
    }

    private void Update()
    {
        if (isLocalPlayer && !controllersLinked)
        {
            LinkLocalSteamVRPlayerObjectToLocalNetworkPlayer();
            controllersLinked = true;
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            LinkLocalSteamVRPlayerObjectToLocalNetworkPlayer();
            Local = this;
            LocalPlayerRootTransform = FindObjectOfType<Player>().transform;

			gameObject.name += " LOCAL " + playerName;
        }
    }

    [ClientRpc]
    internal void RpcTakeHit(NetworkIdentity shotByPlayerId)
    {
        Die(shotByPlayerId); // Player dies in 1 hit for paintball.
    }


    private void Die(NetworkIdentity killedByPlayerId)
    {
        NetworkPlayer killedByPlayer = killedByPlayerId.GetComponent<NetworkPlayer>();

        PlayKillAudio(killedByPlayer);

        Respawn();
    }

    private void PlayKillAudio(NetworkPlayer killedByPlayer)
    {
        if (IsOnLocalPlayersTeam)
        {
            SimpleAudioController.Instance.PlayAtPosition(AudioEventId.Oww, transform.position);
        }
        else if (killedByPlayer == NetworkPlayer.Local)
        {
            SimpleAudioController.Instance.PlayAtPosition(AudioEventId.NiceShotSoldier, transform.position);
        }
        else
        {
            SimpleAudioController.Instance.PlayAtPosition(AudioEventId.WeGotOne, transform.position);
        }
    }

    public void Respawn()
    {
        if (isLocalPlayer)
        {
            int myTeam = playerTeam.Team;
            var respawnPoint = FindObjectOfType<RespawnPointController>().GetRandomRespawnPointForTeam(myTeam);
            
            LocalPlayerRootTransform.position = respawnPoint.transform.position;

            Debug.Log(gameObject.name + " respawning at point " + respawnPoint.gameObject.name);
        }
    }

    private void LinkLocalSteamVRPlayerObjectToLocalNetworkPlayer()
    {
        var hands = FindObjectsOfType<Hand>();

        var leftHand = hands.FirstOrDefault(t => t.startingHandType == Hand.HandType.Left);
        var rightHand = hands.FirstOrDefault(t => t.startingHandType == Hand.HandType.Right);

        if (leftHand == null)
            throw new Exception("Left Hand not found.  Make sure the controller is turned on and the Local Player prefab is configured with a left hand.");
        if (rightHand == null)
            throw new Exception("Right Hand not found.  Make sure the controller is turned on and the Local Player prefab is configured with a left hand.");

        networkLeftHand.SetHand(leftHand);
        networkRightHand.SetHand(rightHand);
        networkHead.SetTransform(FindObjectOfType<SteamVR_Camera>().transform);
    }

    public PlayerTransformData GetLocalPlayerTransformData()
    {
        return new PlayerTransformData()
        {
            HeadPosition = networkHead.LocalPlayerPosition,
            LeftHandPosition = networkLeftHand.LocalPlayerPosition,
            RightHandPosition = networkRightHand.LocalPlayerPosition,

            HeadRotation = networkHead.LocalPlayerRotation,
            LeftHandRotation = networkLeftHand.LocalPlayerRotation,
            RightHandRotation = networkRightHand.LocalPlayerRotation,
        };
    }

    public void SetTransformsToNetworkValues(PlayerTransformData playerTransformData)
    {
        transform.position = playerTransformData.Position;
        transform.rotation = playerTransformData.Rotation;

        networkLeftHand.transform.position = playerTransformData.LeftHandPosition;
        networkLeftHand.transform.rotation = playerTransformData.LeftHandRotation;

        networkRightHand.transform.position = playerTransformData.RightHandPosition;
        networkRightHand.transform.rotation = playerTransformData.RightHandRotation;

        networkHead.transform.position = playerTransformData.HeadPosition;
        networkHead.transform.rotation = playerTransformData.HeadRotation;
    }

}