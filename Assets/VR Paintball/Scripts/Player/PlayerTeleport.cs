using UnityEngine;

[RequireComponent(typeof(NetworkPlayer))]
public class PlayerTeleport : MonoBehaviour
{
    [SerializeField]
    private float maxTeleportDistance = 5f;
    [SerializeField]
    private float teleportRechargePerSecond = 2.5f;

    private NetworkPlayer networkPlayer;

    public float AvailableTeleportDistance { get; private set; }

    public static PlayerTeleport Local;

    private void Awake()
    {
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void Update()
    {
        if (networkPlayer.isLocalPlayer)
        {
            if (Local == null)
                Local = this;

            AvailableTeleportDistance += Time.deltaTime * teleportRechargePerSecond;
            AvailableTeleportDistance = Mathf.Min(AvailableTeleportDistance, maxTeleportDistance);
        }
    }


    internal void HandleTeleportCompleted()
    {
        AvailableTeleportDistance = 0f;
    }
}