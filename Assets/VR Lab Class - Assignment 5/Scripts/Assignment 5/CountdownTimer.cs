using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TextMeshPro timerText;
    public GameObject gameOverPanel;
    public Button restartButton;
    public GameObject balloonSpawner;

    private bool isGameOver = false;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }
    void Update()
    {
        if (!isGameOver && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            
            if(timeRemaining < 0)
            {
                timeRemaining = 0;
            }
            UpdateTimerDisplay();
        }
        else if (!isGameOver)
        {
            GameOver();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Debug.Log("Game over! Time ended!");

        //Time.timeScale = 0;

        if(balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);
        }

        GameObject[] balloons = GameObject.FindGameObjectsWithTag("Balloon");
        foreach (GameObject balloon in balloons)
        {
            Destroy(balloon);
        }

    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
