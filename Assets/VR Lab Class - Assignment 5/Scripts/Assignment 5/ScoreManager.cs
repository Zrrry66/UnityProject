using UnityEngine;
using TMPro; 
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance; // Instance
    public TextMeshPro scoreText; // binding TextMeshPro component
    
    //private int score = 0; // store current score
    private NetworkVariable<int> score = new NetworkVariable<int>(0);

    void Awake()
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

   public override void OnNetworkSpawn()
    {
        if (IsClient) 
        {
            score.OnValueChanged += (oldScore, newScore) => UpdateScoreUI();
        }
    }

     public void AddScore(int points)
    {
        if (IsOwner) // 只有本地客户端可以请求加分
        {
            AddScoreRpc(points);
        }
    }

    [Rpc(SendTo.Server)]
    public void AddScoreRpc(int points)
    {
        score.Value += points; // 服务器端修改，自动同步给所有客户端
    }

     public void ResetScore()
    {
        if (IsServer) score.Value = 0;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.Value.ToString();
    }

    public int GetScore()
    {
        return score.Value;
    }

    /*void Start()
    {
        UpdateScoreUI();
    }
  

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public int GetScore()
    {
        return score;
    }
    */
}
