using System;
using UnityEngine;
using UnityEngine.Networking;

public class GameStateController : NetworkBehaviour
{
    public static GameStateController Instance { get; private set; }

    private GameState CurrentGameState { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameState controllers active, only one should exist.");
            return;
        }
        else
        {
            Instance = this;
        }

        SetGameState(new GameStateWaitingForHost());
    }

    public void SetGameState(GameState gameState)
    {
        gameObject.SetActive(true);
        if (CurrentGameState != null)
        {
            try
            {
                CurrentGameState.ExitState();
            }
            catch (Exception ex)
            {
                gameObject.name = "[GameStateController] - EXIT FAILED - " + CurrentGameState.GetType().Name;
                throw ex;
            }
        }

        CurrentGameState = gameState;

        if (CurrentGameState != null)
        {
            try
            {
                CurrentGameState.EnterState();
            }
            catch (Exception ex)
            {
                gameObject.name = "[GameStateController] - ENTER FAILED - " + gameState.GetType().Name;
                throw ex;
            }
        }

        gameObject.name = "[GameStateController] - " + gameState.GetType().Name;
    }

    private void Update()
    {
        if (CurrentGameState != null)
        {
            CurrentGameState.Tick();
        }
    }
}