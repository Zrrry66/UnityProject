using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60f;  // 倒计时时间
    public TextMeshPro timerText;      // UI 倒计时显示
    //public GameObject balloonSpawner;  // 气球生成器

    private bool isRunning = false;
    private GameManager gameManager;   // 引用 GameManager

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // 找到 GameManager
    }

    void Update()
    {
        if (isRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;
            UpdateTimerDisplay();
        }
        else if (isRunning) // 时间到了
        {
            isRunning = false;
            gameManager.GameOver(); // 通知 GameManager 游戏结束
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // **开始倒计时（由 GameManager 调用）**
    public void StartTimer(float gameTime)
    {
        timeRemaining = gameTime;
        isRunning = true;
    }

    // **停止倒计时**
    public void StopTimer()
    {
        isRunning = false;
    }
}


/*using UnityEngine;
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

        Time.timeScale = 0;

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

*/