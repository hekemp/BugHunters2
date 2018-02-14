using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PaintballNetworkManager : NetworkManager
{
    public event Action<NetworkConnection> OnClientConnectedEvent = delegate { };

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("OnMatchCreate");

        base.OnMatchCreate(success, extendedInfo, matchInfo);

        GameStateController.Instance.SetGameState(new GameStateWaitingForPlayers());

        GameStateController.Instance.gameObject.SetActive(true);
    }

	internal void JoinGame(MatchConnectionInfo matchConnectionInfo)
	{
		if (matchConnectionInfo.IsLan)
		{
			networkAddress = matchConnectionInfo.Address;
			StartClient();
			FindObjectOfType<NetworkManagerUI>().gameObject.SetActive(false);
		}
		else
		{
			if (matchMaker == null)
				StartMatchMaker();

			matchMaker.JoinMatch(matchConnectionInfo.NetworkId, "", "", "", 0, 0, HandleJoinedMatch);
		}
	}

	internal void StartLanHost()
	{
		StartHost();
		FindObjectOfType<PaintballNetworkDiscovery>().StartBroadcast();
	}

	private void HandleJoinedMatch(bool success, string extendedInfo, MatchInfo responseData)
	{
		Debug.Log("Joined Match Success: " + success);
		StartClient(responseData);
		FindObjectOfType<NetworkManagerUI>().gameObject.SetActive(false);
	}

	/// <summary>
	/// When the host starts a game, we begin in a waiting for players state.
	/// Once enough players join, the gamestatecontroller will begin the actual match.
	/// </summary>
	public override void OnStartHost()
    {
        Debug.Log("OnStartHost");

        base.OnStartHost();

        GameStateController.Instance.SetGameState(new GameStateWaitingForPlayers());
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        GameStateController.Instance.SetGameState(new GameStateWaitingForHost());
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnectedEvent(conn);
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
    }
}
