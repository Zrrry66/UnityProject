using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{

     public static BalloonSpawner Instance;

    [System.Serializable]
    public class BalloonType
    {
        public GameObject prefab;
        public float spawnProbability;
    }

    public BalloonType[] balloonTypes;
    public Transform spawnArea; // Center of the sapwn area
    public Vector3 spawnRange = new Vector3(5f, 2f, 5f); // Spawn range
    public float spawnInterval = 2.0f;

    private void Awake()
    {
        Instance = this; // let Balloon.cs access spawnRange
    }
    
    private void Start()
    {
        InvokeRepeating(nameof(SpawnBalloon), 1f, spawnInterval);
    }

   void OnDrawGizmos()
{
    if (spawnArea == null) return;

    Gizmos.color = Color.green;
    Vector3 center = spawnArea.position;
    Vector3 size = spawnRange * 2; // spawn range spread

    Gizmos.DrawWireCube(center, size);
}
 void SpawnBalloon()
    {
        if (balloonTypes.Length == 0) return;

        // Calculate probability
        float totalProbability = 0f;
        foreach (var balloon in balloonTypes)
        {
            totalProbability += balloon.spawnProbability;
        }

        float randomPoint = Random.value * totalProbability;
        float cumulativeProbability = 0f;
        GameObject selectedBalloon = null;

        foreach (var balloon in balloonTypes)
        {
            cumulativeProbability += balloon.spawnProbability;
            if (randomPoint <= cumulativeProbability)
            {
                selectedBalloon = balloon.prefab;
                break;
            }
        }

        if (selectedBalloon != null)
        {
            // Spawn randomly
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnArea.position.x - spawnRange.x, spawnArea.position.x + spawnRange.x),
                spawnArea.position.y,
                Random.Range(spawnArea.position.z - spawnRange.z, spawnArea.position.z + spawnRange.z)
            );

            Instantiate(selectedBalloon, spawnPosition, Quaternion.identity);
        }
    }
}
