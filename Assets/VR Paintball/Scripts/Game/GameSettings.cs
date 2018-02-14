using UnityEngine;

/// <summary>
/// This class holds settings for our game.
/// </summary>
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [SerializeField]
    [Range(1, 8)]
    private int minimumPlayersToStartRound = 2;

    public int MinimumPlayersToStartRound { get { return minimumPlayersToStartRound; } }

    [SerializeField]
    private bool allowTeamKills = false;

    public bool AllowTeamKills { get { return allowTeamKills; } }

    private void Awake()
    {
        Instance = this;
    }
}