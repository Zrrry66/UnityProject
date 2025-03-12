using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

public class GameManager : NetworkBehaviour
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

    //private string selectedWeapon = null;
 // Modified: use network variable to store selected weapon
    private NetworkVariable<string> selectedWeapon = new NetworkVariable<string>();



    [Header("Dart Settings")]
public GameObject dartPrefab; // 这里添加 dartPrefab

    
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

      selectedWeapon.Value = weapon; // Modified: set network variable

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
GameObject g1 = Instantiate(gunPrefab, gunSpawnPoint1.position, gunSpawnPoint1.rotation);
        Rigidbody rb1 = g1.GetComponent<Rigidbody>();
        if (rb1 != null)
        {
            rb1.isKinematic = true;  // Make gun stop in the air
            rb1.useGravity = false;
            rb1.velocity = Vector3.zero;
            rb1.angularVelocity = Vector3.zero;
        }

        NetworkObject netObj1 = g1.GetComponent<NetworkObject>();
        if (netObj1 != null)
        {
            netObj1.Spawn();
        }
                
        }
        if (gunSpawnPoint2 != null)
        {
GameObject g2 = Instantiate(gunPrefab, gunSpawnPoint2.position, gunSpawnPoint2.rotation);
        Rigidbody rb2 = g2.GetComponent<Rigidbody>();
        if (rb2 != null)
        {
            rb2.isKinematic = true;  // Make gun stop in the air
            rb2.useGravity = false;
            rb2.velocity = Vector3.zero;
            rb2.angularVelocity = Vector3.zero;
        }

        NetworkObject netObj2 = g2.GetComponent<NetworkObject>();
        if (netObj2 != null)
        {
            netObj2.Spawn();
        }
            
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

    // **Spawn DartSpawner 1**
    if (dartSpawnPoint1 != null)
    {
        GameObject d1 = Instantiate(dartSpawnerPrefab, dartSpawnPoint1.position, dartSpawnPoint1.rotation);
        DartSpawner spawner1 = d1.GetComponent<DartSpawner>();

        if (spawner1 != null)
        {
            spawner1.spawnPoint = dartSpawnPoint1; 
            Debug.Log("DartSpawner 1 assigned to spawn at " + dartSpawnPoint1.position);

            spawner1.SpawnNewDart(); 
        }
        else
        {
            Debug.LogWarning("DartSpawner script not found on spawned object 1!");
        }
 	NetworkObject netObj1 = d1.GetComponent<NetworkObject>();
            if (netObj1 != null)
            {
                netObj1.Spawn(); // Modified: network spawn dart spawner
            }
    }

    // **Spawn DartSpawner 2**
    if (dartSpawnPoint2 != null)
    {
        GameObject d2 = Instantiate(dartSpawnerPrefab, dartSpawnPoint2.position, dartSpawnPoint2.rotation);
        DartSpawner spawner2 = d2.GetComponent<DartSpawner>();

        if (spawner2 != null)
        {
            spawner2.spawnPoint = dartSpawnPoint2;
            Debug.Log("DartSpawner 2 assigned to spawn at " + dartSpawnPoint2.position);

            spawner2.SpawnNewDart();
        }
        else
        {
            Debug.LogWarning("DartSpawner script not found on spawned object 2!");
        }
NetworkObject netObj2 = d2.GetComponent<NetworkObject>();
            if (netObj2 != null)
            {
                netObj2.Spawn(); // Modified: network spawn dart spawner
            }
    }
}

private void ClearWeapons()
{
    GameObject[] guns = GameObject.FindGameObjectsWithTag("Gun");
    foreach (GameObject gun in guns)
    {
        NetworkObject netObj = gun.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Despawn(); // Modified: remove network object from all clients
        }
        else
        {
            Destroy(gun);
        }
    }
    GameObject[] dartspawners = GameObject.FindGameObjectsWithTag("DartSpawner");
    foreach (GameObject dartspawner in dartspawners)
    {
        NetworkObject netObj = dartspawner.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Despawn();
        }
        else
        {
            Destroy(dartspawner);
        }
    }

    GameObject[] darts = GameObject.FindGameObjectsWithTag("Dart");
    foreach (GameObject dart in darts)
    {
        NetworkObject netObj = dart.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Despawn();
        }
        else
        {
            Destroy(dart);
        }
    }
}



    private void StartActualGame()
    {
        //ScoreManager.Instance.ResetScore();
        if (IsServer)
        {
            ScoreManager.Instance.ResetScore(); 
        }

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
        // Modified: if not listening, start host (only once)
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
            raycaster.ReactivateRay();
        }

        if (balloonSpawner != null)
        {
            balloonSpawner.SetActive(false);
        }

            // 关键：销毁武器
    ClearWeapons();

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

        ClearWeapons();

        // **重新生成武器**
    if (selectedWeapon.Value == "Gun")
    {
        SpawnTwoGuns();
    }
    else if (selectedWeapon.Value == "Dart")
    {
        SpawnTwoDartSpawners();
    }


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
            raycaster.ReactivateRay();
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
           ClearWeapons();
        
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