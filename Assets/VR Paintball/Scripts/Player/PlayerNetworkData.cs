using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkData : NetworkBehaviour
{
	//[SyncVar]
	private string playerName;

	public string PlayerName { get { return playerName; } }

	public static PlayerNetworkData Local { get; private set; }

	public override void OnStartLocalPlayer()
	{
		Local = this;
		base.OnStartLocalPlayer();

		CmdSetPlayerName(LocalPlayerNameInputField.LocalPlayerName);
		CmdRequestAllPlayerData();
	}

	[ContextMenu("Request!")]
	public void ForceRequest()
	{
		CmdRequestAllPlayerData();
	}

	[Command]
	public void CmdSetPlayerName(string newName)
	{
		playerName = newName;
	}

	[Command]
	private void CmdRequestAllPlayerData()
	{
		foreach (var playerData in FindObjectsOfType<PlayerNetworkData>())
			playerData.SendPlayerName();
	}

	private void SendPlayerName()
	{
		RpcSetPlayerName(this.playerName);
	}

	[ClientRpc]
	public void RpcSetPlayerName(string newPlayerName)
	{
		playerName = newPlayerName;
		gameObject.name += playerName;

		var floatingNameTag = gameObject.GetComponentInChildren<PlayerFloatingNameTag>();
		if (floatingNameTag != null)
			floatingNameTag.SetNameText(playerName);

		Debug.Log(gameObject.name + " Name=" + playerName);
	}
}