using UnityEngine;
using TMPro;
using Unity.Netcode;

public class CountdownTimer : NetworkBehaviour
{
    // change timeRemaining to a network variable with initial value 60f
    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>(60f);
    public TextMeshPro timerText; 

    private bool isRunning = false;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Find GameManager

		// Subscribe to changes so all clients update UI
        timeRemaining.OnValueChanged += (oldValue, newValue) => UpdateTimerDisplay();
    }

    void Update()
    {
        // Only server updates the countdown
        if (IsServer && isRunning && timeRemaining.Value > 0)
        {
            timeRemaining.Value -= Time.deltaTime;
            if (timeRemaining.Value < 0) timeRemaining.Value = 0;
            // UI updated via OnValueChanged event
        }
        else if (IsServer && isRunning) // Time ended on server
        {
            isRunning = false;
            gameManager.GameOver();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining.Value / 60);
        int seconds = Mathf.FloorToInt(timeRemaining.Value % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


	// Only server should start timer and update network variable
    public void StartTimer(float gameTime)
    {
        if (IsServer)
        {
            timeRemaining.Value = gameTime;
            isRunning = true;
        }
    }

    // Stop timer
    public void StopTimer()
    {
        isRunning = false;
    }
}