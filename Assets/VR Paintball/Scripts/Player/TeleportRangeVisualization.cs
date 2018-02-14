using UnityEngine;

public class TeleportRangeVisualization : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If the texture doesn't line up exactly with the range, use this to adjust the texture scale.")]
    private float teleportIndicatorScaleFactor = 2f;

    private PlayerTeleport playerTeleport;
    private Transform bodyTransform;

    private void Start()
    {
        playerTeleport = GetComponentInParent<PlayerTeleport>();
        bodyTransform = transform.parent.GetComponentInChildren<PlayerBody>().transform;

        if (GetComponentInParent<NetworkPlayer>().isLocalPlayer == false)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        transform.localScale = new Vector3(
            playerTeleport.AvailableTeleportDistance * teleportIndicatorScaleFactor,
            playerTeleport.AvailableTeleportDistance * teleportIndicatorScaleFactor,
            1f);

        transform.position = new Vector3(bodyTransform.position.x, 0.01f, bodyTransform.position.z);
    }
}