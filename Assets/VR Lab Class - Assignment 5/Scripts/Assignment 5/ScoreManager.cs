using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance; // Instance
    public TextMeshPro scoreText; // binding TextMeshPro component
    //private int score = 0; // store current score
    // Modified: change score to network variable with initial value 0
    private NetworkVariable<int> score = new NetworkVariable<int>(0);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Subscribe to score changes to update UI on all clients
            score.OnValueChanged += (oldValue, newValue) => UpdateScoreUI();
        }
        else
        {
            Destroy(gameObject);
        }
    } 
    void Start()
    {
        UpdateScoreUI();
    }
  

    public void AddScore(int points)
    {
        if (IsServer)
        {
            score.Value += points; // Modified: update network variable
        }
        //UpdateScoreUI();
    }

    public void ResetScore()
    {
        if (IsServer)
        {
            score.Value = 0; // Modified: update network variable
        }
        //UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
			scoreText.text = score.Value.ToString(); // Modified: use network variable value
        }
    }

    public int GetScore()
    {
        return score.Value;
    }

}
