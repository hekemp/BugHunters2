using UnityEngine;
using UnityEngine.Networking;

public class GameStateWaitingForPlayers : GameState
{
    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void Tick()
    {
        if (GameStateController.Instance.isServer)
        {
            if (networkManager.numPlayers >= GameSettings.Instance.MinimumPlayersToStartRound)
            {
                SetState(new GameStatePreGameCountdown());
            }
        }
    }
}