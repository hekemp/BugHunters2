using UnityEngine;
using UnityEngine.Networking;

public abstract class GameState
{
    protected NetworkManager networkManager;

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void Tick();

    public GameState()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
    }

    protected void SetState(GameState gameState)
    {
        GameStateController.Instance.SetGameState(gameState);
    }
}
