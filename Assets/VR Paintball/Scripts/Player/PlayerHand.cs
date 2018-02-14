using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerHand : MonoBehaviour
{
    private NetworkPlayer player;

    private Transform localHandTransform;

    public Hand LocalHand { get; private set; }

    public Vector3 LocalPlayerPosition { get { return localHandTransform.position; } }
    public Quaternion LocalPlayerRotation { get { return localHandTransform.rotation; } }

    private void Awake()
    {
        player = GetComponentInParent<NetworkPlayer>();
    }

    public void SetHand(Hand hand)
    {
        LocalHand = hand;
        localHandTransform = hand.transform;
    }

    private void Update()
    {
        UpdateNetworkHandForLocalClient();
    }

    private void UpdateNetworkHandForLocalClient()
    {
        if (localHandTransform != null)
        {
            transform.position = LocalPlayerPosition;
            transform.rotation = LocalPlayerRotation;
        }
    }
}