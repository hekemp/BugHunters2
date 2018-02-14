using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkManagerUI : MonoBehaviour
{
    private MatchInfo matchInfo;
    private PaintballNetworkManager networkManager;

	private float refreshInterval = 5f;
	private float nextRefreshTime;

	private void OnEnable()
    {
        networkManager = FindObjectOfType<PaintballNetworkManager>();
    }

	private void Update()
	{
		if (Time.time >= nextRefreshTime)
		{
			nextRefreshTime = Time.time + refreshInterval;
			RefreshMatches();
		}
	}

	public void StartLanHost()
    {
        Debug.Log("Starting LAN Server");
        networkManager.StartLanHost();
        gameObject.SetActive(false);
    }

    public void StartMatchMakerServer()
    {
		string matchName = string.Format("{0}'s game", LocalPlayerNameInputField.LocalPlayerName);

		Debug.Log("Starting Online Server (with matchmaker)");
        networkManager.StartMatchMaker();
        networkManager.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnMatchCreated);
        gameObject.SetActive(false);
    }

    public void RefreshMatches()
    {
		if (networkManager.matchMaker == null)
			networkManager.StartMatchMaker();

		networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, HandleMatches);
    }

    private void OnMatchCreated(bool success, string extendedInfo, MatchInfo responseData)
    {
        networkManager.StartHost(responseData);
       
        GameStateController.Instance.SetGameState(new GameStateWaitingForPlayers());
    }

    private void OnJoinedMatch(bool success, string extendedInfo, MatchInfo responseData)
    {
        
        Debug.Log("Joined Match");
        // Joined Match
    }

    private void HandleMatches(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        Debug.Log("Got Matches " + responseData.Count);

		GameListController.AddMatchmakerMatches(responseData);
    }

    public void JoinLanGame()
    {
        matchInfo = new MatchInfo();
    }
}
