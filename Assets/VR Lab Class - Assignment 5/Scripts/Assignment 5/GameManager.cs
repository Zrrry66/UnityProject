using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject startPanel;
    public GameObject gameOverPanel;
    public Button startButton;
    public Button restartButton;
    public Button exitButton;
    public CountdownTimer countdownTimer;
    public GameObject balloonSpawner;
    public GameObject player; // XR character control
    public float gameTime = 60f;

    // New: Weapon selection UI
    public GameObject weaponSelectionPanel;
    public TMP_Text weaponSelectionText;
    public Button gunButton;
    public Button dartButton;
    public GameObject gunObject;  // Assign in Unity Inspector
    public GameObject dartSpawner; // Assign in Unity Inspector

    private enum GameState { WaitingToStart, Playing, GameOver }
    private GameState currentState = GameState.WaitingToStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startButton.onClick.AddListener(ShowWeaponSelection);  // Modified: Show weapon selection first
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);

        ShowStartPanel();
    }

    void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        weaponSelectionPanel.SetActive(false);  // Hide weapon selection panel
        balloonSpawner.SetActive(false); // Ensure balloon spawner is inactive before game starts
        ScoreManager.Instance.ResetScore();
        currentState = GameState.WaitingToStart;
    }

    // Show weapon selection after clicking Start
    private void ShowWeaponSelection()
    {
        startPanel.SetActive(false);
        weaponSelectionPanel.SetActive(true);
        weaponSelectionText.text = "Please choose your weapon";

        // Bind weapon selection button events
        gunButton.onClick.RemoveAllListeners();
        dartButton.onClick.RemoveAllListeners();
        gunButton.onClick.AddListener(() => SelectWeapon("Gun"));
        dartButton.onClick.AddListener(() => SelectWeapon("Dart"));
    }

    private void SelectWeapon(string weapon)
    {
        weaponSelectionPanel.SetActive(false); // Hide weapon selection panel

        if (weapon == "Gun")
        {
            if (dartSpawner != null)
            {
                dartSpawner.SetActive(false); // Disable DartSpawner
            }
        }
        else if (weapon == "Dart")
        {
            if (gunObject != null)
            {
                gunObject.SetActive(false); // Hide Gun
            }
        }

        StartActualGame(); // Continue original game logic
    }

    // This method retains the original StartGame() logic
    private void StartActualGame()
    {
        gameOverPanel.SetActive(false);
        ScoreManager.Instance.ResetScore();
        countdownTimer.StartTimer(gameTime);
        currentState = GameState.Playing;

        // Deactivate RayCaster
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
                spawnerScript.StartSpawning();
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

        // Reactivate RayCaster
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
            raycaster.enabled = true;
        }

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);
        }

        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        // Deactivate RayCaster
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
        }

        if (player != null && player.GetComponent<CharacterController>() != null)
        {
            player.GetComponent<CharacterController>().enabled = true;
        }

        // Clear existing balloons
        ClearBalloons();

        // Reset game state
        ScoreManager.Instance.ResetScore();
        countdownTimer.StartTimer(gameTime);

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(true);

            BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.StartSpawning();
            }
        }

        gameOverPanel.SetActive(false);
        currentState = GameState.Playing;
    }

    public void ExitGame()
    {
        ShowStartPanel();

        // Reactivate RayCaster
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
            raycaster.enabled = true;
        }

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);

            BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.StopSpawning();
            }
        }

        ClearBalloons();
        Time.timeScale = 1;

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

    public bool IsGameRunning()
    {
        return currentState == GameState.Playing;
    }
}

/*this works. need to add weapon select panel
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        balloonSpawner.SetActive(false); // Make sure balloons don't spawn before game starts
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

        // **Deactivate ray after clicking on the button**
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
                spawnerScript.StartSpawning(); 
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

        // **Reactivate ray when game ends**
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay(); // Ensure it starts OFF
            raycaster.enabled = true; // Allow reactivation
        }

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);
        }

        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        // **Deactivate ray after clicking on the button**
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
        }

        if (player != null && player.GetComponent<CharacterController>() != null)
        {
            player.GetComponent<CharacterController>().enabled = true;
        }

        // **Clear existing balloons**
        ClearBalloons();

        // **Reset game state**
        ScoreManager.Instance.ResetScore();
        countdownTimer.StartTimer(gameTime);

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(true);

            BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.StartSpawning();
            }
        }

        gameOverPanel.SetActive(false);
        currentState = GameState.Playing;
    }

    public void ExitGame()
    {
        ShowStartPanel();

        // **Reactivate ray when returning to menu**
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
            raycaster.enabled = true;
        }

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);

            BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.StopSpawning();
            }
        }

        ClearBalloons();
        Time.timeScale = 1;

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

    // **This function allows other scripts to check if the game is running**
    public bool IsGameRunning()
    {
        return currentState == GameState.Playing;
    }
}
*/

/*this works. need to add singleton
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
*/