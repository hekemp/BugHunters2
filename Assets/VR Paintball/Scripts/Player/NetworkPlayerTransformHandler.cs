using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkPlayer))]
public class NetworkPlayerTransformHandler : NetworkBehaviour
{
    [SerializeField]
    private int networkUpdatesPerSecond = 30;
    
    private NetworkPlayer networkPlayer;

    private PlayerTransformData mostRecentPlayerTransformData;

    private float networkSendTimer;
    
    private void Awake()
    {
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    [Command]
    private void CmdSendNetworkPlayerTransformDataToServer(byte[] data)
    {
        mostRecentPlayerTransformData = networkPlayerTransformWriter.FromBytes(data);
        RpcSendNetworkPlayerTransformDataToClients(data);
    }

    [ClientRpc]
    private void RpcSendNetworkPlayerTransformDataToClients(byte[] data)
    {
        if (isLocalPlayer)
            return;

        mostRecentPlayerTransformData = networkPlayerTransformWriter.FromBytes(data);
        networkPlayer.SetTransformsToNetworkValues(mostRecentPlayerTransformData);
    }

    [Client]
    private void Update()
    {
        if (ShouldSendPlayerTransformDataToServer())
        {
            networkSendTimer = 1f / (float)networkUpdatesPerSecond;

            var playerTransformDataToSendServer = networkPlayer.GetLocalPlayerTransformData();

            var data = networkPlayerTransformWriter.GetBytes(playerTransformDataToSendServer);

            CmdSendNetworkPlayerTransformDataToServer(data);
        }
    }

    private NetworkPlayerTransformWriter networkPlayerTransformWriter = new NetworkPlayerTransformWriter();

    [Client]
    private bool ShouldSendPlayerTransformDataToServer()
    {
        if (!isLocalPlayer)
            return false;

        networkSendTimer -= Time.deltaTime;
        return networkSendTimer <= 0f;
    }
}