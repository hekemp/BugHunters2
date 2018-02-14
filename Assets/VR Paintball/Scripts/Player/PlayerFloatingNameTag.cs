using UnityEngine;
using UnityEngine.UI;

public class PlayerFloatingNameTag : MonoBehaviour
{
	[SerializeField]
	private float nametagOffset = 0.5f;

	private Text nameText;
	private Transform headTransform;
	private NetworkPlayer networkPlayer;

	private void Awake()
	{
		networkPlayer = GetComponentInParent<NetworkPlayer>();
		nameText = GetComponentInChildren<Text>();
		headTransform = transform.root.GetComponentInChildren<PlayerHead>().transform;
	}

	public void SetNameText(string text)
	{
		nameText.text = text;

		if (networkPlayer.isLocalPlayer)
			gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!networkPlayer.isLocalPlayer)
		{
			transform.position = headTransform.position + (Vector3.up * nametagOffset);
			transform.LookAt(PlayerHead.LocalTransform);
		}
	}
}