using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script handles assigning players to teams.  
/// It interacts with the PlayerTeam component on the player prefab to tell the player which team it should be on.
/// </summary>
public class TeamController : NetworkBehaviour
{
    public static TeamController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Team Controllers active, only one should exist.");
            return;
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField]
    [Range(1, 4)]
    private int totalTeamCount = 2;

    private PaintballNetworkManager networkManager;

    private List<PlayerTeam> allPlayers = new List<PlayerTeam>();

    private void Start()
    {
        networkManager = FindObjectOfType<PaintballNetworkManager>();
    }

    public void RequestTeam(PlayerTeam playerTeam)
    {
        allPlayers.Add(playerTeam);

        var teamForPlayer = GetTeamNeedingPlayers();
        playerTeam.RpcSetTeam(teamForPlayer);
    }

    private int GetTeamNeedingPlayers()
    {
        allPlayers = FindObjectsOfType<PlayerTeam>().ToList();

        Dictionary<int, int> teamCounts = new Dictionary<int, int>();
        for (int teamNumber = 1; teamNumber <= totalTeamCount; teamNumber++)
        {
            teamCounts[teamNumber] = allPlayers.Count(t => t.Team == teamNumber);
        }

        return teamCounts.OrderBy(t=> t.Value).First().Key;
    }
}