using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public Button startButton;
    public Button restartButton;
    public Button exitButton;
    public CountdownTimer countdownTimer;
    public GameObject balloonSpawner;
    public GameObject player; // XR 角色控制
    public float gameTime = 60f;

    private enum GameState { WaitingToStart, Playing, GameOver }
    private GameState currentState = GameState.WaitingToStart;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);

        ShowStartPanel();
    }

    void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        balloonSpawner.SetActive(false); // 确保游戏未开始时气球不生成
        ScoreManager.Instance.ResetScore();
        currentState = GameState.WaitingToStart;
    }

public void StartGame()
{
    startPanel.SetActive(false);
    gameOverPanel.SetActive(false);
    ScoreManager.Instance.ResetScore();
    countdownTimer.StartTimer(gameTime);
    currentState = GameState.Playing;
    
    
    //deactivate ray after cliking on button
    RayCaster raycaster = FindObjectOfType<RayCaster>();
    if (raycaster != null)
    {
        raycaster.DeactivateRay();
    }
    
    if (balloonSpawner != null)
    {
        balloonSpawner.SetActive(true);

        BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.StartSpawning(); // **确保 StartGame() 也会重新启动气球生成**
        }
    }

    if (NetworkManager.Singleton.IsServer)
    {
        NetworkManager.Singleton.StartHost();
    }
}


    public void GameOver()
    {
        currentState = GameState.GameOver;
        gameOverPanel.SetActive(true);
        
        // **停止气球生成**
        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);
        }

        // **暂停游戏**
        Time.timeScale = 0;
    }

public void RestartGame()
{
    Time.timeScale = 1;

    //deactivate ray after cliking on button
    RayCaster raycaster = FindObjectOfType<RayCaster>();
    if (raycaster != null)
    {
        raycaster.DeactivateRay();
    }


    if (player != null && player.GetComponent<CharacterController>() != null)
    {
        player.GetComponent<CharacterController>().enabled = true;
    }

    // **清除现有气球**
    ClearBalloons();

    // **重置游戏状态**
    ScoreManager.Instance.ResetScore();
    countdownTimer.StartTimer(gameTime);
    
    if (balloonSpawner != null)
    {
        balloonSpawner.SetActive(true);
        
        BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.StartSpawning(); // **重新开始生成气球**
        }
    }

    gameOverPanel.SetActive(false);
    currentState = GameState.Playing;
}

public void ExitGame()
{
    ShowStartPanel();


    //deactivate ray after cliking on button
    RayCaster raycaster = FindObjectOfType<RayCaster>();
    if (raycaster != null)
    {
        raycaster.DeactivateRay();
    }

    

    // **彻底停止气球生成**
    if (balloonSpawner != null)
    {
        balloonSpawner.SetActive(false);
        
        BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.StopSpawning(); // **停止生成**
        }
    }

    // **清除所有气球**
    ClearBalloons();

    // **恢复时间**
    Time.timeScale = 1;

    // **恢复玩家控制**
    if (player != null && player.GetComponent<CharacterController>() != null)
    {
        player.GetComponent<CharacterController>().enabled = true;
    }
}


    private void ClearBalloons()
    {
        GameObject[] balloons = GameObject.FindGameObjectsWithTag("Balloon");
        foreach (GameObject balloon in balloons)
        {
            Destroy(balloon);
        }
    }
}
