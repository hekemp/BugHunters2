using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerText : MonoBehaviour
{
    private Text timerText;

    private float timeRemaining;
    
    private void Awake()
    {
        timerText = GetComponent<Text>();
    }

    public void StartCountdown(int secondsToCountDown)
    {
        timeRemaining = secondsToCountDown;

        timerText.text = secondsToCountDown.ToString();
        timerText.enabled = true;
    }
    
    private void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            StopCountdown();
        }
        else
        {
            timerText.text = string.Format("{0:0}", timeRemaining);
        }
    }

    private void StopCountdown()
    {
        timerText.enabled = false;
    }
}
