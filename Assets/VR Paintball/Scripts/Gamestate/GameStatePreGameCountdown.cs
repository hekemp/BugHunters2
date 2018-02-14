using System;
using UnityEngine;

public class GameStatePreGameCountdown : GameState
{
    private int timeToWait = 5;

    private float timeRemaining;

    public override void EnterState()
    {
        AssignTeams();
        // Move players to starting points (something that needs to happen when they die too so do it out of here)
        // Lock player movement
        // Start visual timer
        // Countdown and exit state when timer ends

        var countdownTimerText = GameObject.FindObjectOfType<CountdownTimerText>();
        if (countdownTimerText != null)
            countdownTimerText.StartCountdown(timeToWait);

        timeRemaining = timeToWait;

        PlayCountdownAudio();
    }

    private void AssignTeams()
    {
        int teamCount = 2;
        int nextTeam = 1;

        foreach(var player in GameObject.FindObjectsOfType<PlayerTeam>())
        {
            player.RpcSetTeam(nextTeam);
            nextTeam++;
            if (nextTeam > teamCount)
            {
                nextTeam = 1;
            }
        }
    }

    public override void ExitState()
    {
    }

    public override void Tick()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining<= 0)
        {
            SetState(new GameStatePlaying());
        }
    }

    private void PlayCountdownAudio()
    {
        SimpleAudioController.Instance.PlayAtPosition(AudioEventId.Countdown, NetworkPlayer.Local.transform.position);
    }
}