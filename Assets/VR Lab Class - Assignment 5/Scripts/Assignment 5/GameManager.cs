using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 
using VRSYS.Core.Networking;

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
    public float gameTime = 60f;

    // Weapon selection UI
    public GameObject weaponSelectionPanel;
    public TMP_Text weaponSelectionText;
    public Button gunButton;
    public Button dartButton;
    
    [Header("Weapon Objects")]
    //public GameObject gunObject;
    public GameObject gunPrefab;

    public Transform gunSpawnPoint1;
    public Transform gunSpawnPoint2;
    
    public GameObject dartSpawnerPrefab;
    public Transform dartSpawnPoint1;
    public Transform dartSpawnPoint2;

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
        // Buttons
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        // Set weapon button to uninteractable, change it in ShwoWeaponSelection
        if (gunButton != null)
        {
            gunButton.interactable = false;
        }

        if (dartButton != null)
        {
            dartButton.interactable = false;
        }
        
        ShowStartPanel();
    }
    
    void ShowStartPanel()
    {
        // Show Start Panel 
        if (startPanel != null) 
        { 
            startPanel.SetActive(true);
            startButton.interactable = false;
        }
        
        //hide panels
        if (weaponSelectionPanel != null)
        {
            weaponSelectionPanel.SetActive(false); 
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Ensure balloon spawner is inactive before game starts
        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false); 
        }
        
        // Initialize
        ScoreManager.Instance.ResetScore();
        currentState = GameState.WaitingToStart;
    }

    void Update()
    {
        if (NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsHost)
        {
            // Host can click the button
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    private void OnStartButtonClicked()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        ShowWeaponSelection();
    }

    // Show weapon selection after clicking Start
    private void ShowWeaponSelection()
    {
        if (startPanel != null)
        {
             startPanel.SetActive(false);
        }

        if (weaponSelectionPanel != null)
        {
            weaponSelectionPanel.SetActive(true);
        }
        weaponSelectionText.text = "Please choose your weapon";

        // Bind weapon selection button events
        gunButton.onClick.RemoveAllListeners();
        dartButton.onClick.RemoveAllListeners();
        gunButton.onClick.AddListener(() => SelectWeapon("Gun"));
        dartButton.onClick.AddListener(() => SelectWeapon("Dart"));
    }

    private void SelectWeapon(string weapon)
    {
        // Hide weapon selection panel
        if (weaponSelectionPanel != null)
        {
            weaponSelectionPanel.SetActive(false);
        } 

        if (weapon == "Gun")
        {
            SpawnTwoGuns();
        }
        else if (weapon == "Dart")
        {
            SpawnTwoDartSpawners();
        }

        StartActualGame(); // Enter game
    }

    private void SpawnTwoGuns()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (gunPrefab == null)
        {
            Debug.LogWarning("Gun Prefab not assigned!");
            return;
        }

        if (gunSpawnPoint1 != null)
        {
            var g1 = Instantiate(gunPrefab, gunSpawnPoint1.position, gunSpawnPoint1.rotation);
            var netObj1 = g1.GetComponent<NetworkObject>();
            if (netObj1 != null)
                netObj1.Spawn(); 
        }
        if (gunSpawnPoint2 != null)
        {
            var g2 = Instantiate(gunPrefab, gunSpawnPoint2.position, gunSpawnPoint2.rotation);
            var netObj2 = g2.GetComponent<NetworkObject>();
            if (netObj2 != null)
                netObj2.Spawn();
        }
    }

    private void SpawnTwoDartSpawners()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (dartSpawnerPrefab == null)
        {
            Debug.LogWarning("DartSpawner Prefab not assigned!");
            return;
        }

        if (dartSpawnPoint1 != null)
        {
            var d1 = Instantiate(dartSpawnerPrefab, dartSpawnPoint1.position, dartSpawnPoint1.rotation);
            var netObj1 = d1.GetComponent<NetworkObject>();
            if (netObj1 != null)
                netObj1.Spawn();
        }
        if (dartSpawnPoint2 != null)
        {
            var d2 = Instantiate(dartSpawnerPrefab, dartSpawnPoint2.position, dartSpawnPoint2.rotation);
            var netObj2 = d2.GetComponent<NetworkObject>();
            if (netObj2 != null)
                netObj2.Spawn();
        }
    }
    private void StartActualGame()
    {
        ScoreManager.Instance.ResetScore();
        countdownTimer.StartTimer(gameTime);
        currentState = GameState.Playing;

        // Deactivate RayCaster
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
        }

        // Activate balloon spawner
        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(true);

            BalloonSpawner spawnerScript = balloonSpawner.GetComponent<BalloonSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.StartSpawning();
            }
        }
        
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartHost();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

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
        currentState = GameState.Playing;

        // Deactivate RayCaster
        RayCaster raycaster = FindObjectOfType<RayCaster>();
        if (raycaster != null)
        {
            raycaster.DeactivateRay();
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

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        ShowStartPanel();
        
        Time.timeScale = 1;

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
        
        // Set start button to be only clickable for Host
        if (!NetworkManager.Singleton.IsHost && startButton != null)
            startButton.interactable = false;
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