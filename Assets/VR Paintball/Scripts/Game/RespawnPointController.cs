using System.Linq;
using UnityEngine;

public class RespawnPointController : MonoBehaviour
{
    private PlayerStartPoint[] respawnPoints;

    private void Awake()
    {
        respawnPoints = FindObjectsOfType<PlayerStartPoint>();
    }

    public PlayerStartPoint GetRandomRespawnPointForTeam(int teamNumber)
    {
        var availablePoints = respawnPoints.Where(t => t.TeamNumber == teamNumber);

        var selectedPoint = availablePoints
            .OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
            .FirstOrDefault();

        return selectedPoint;
    }
}