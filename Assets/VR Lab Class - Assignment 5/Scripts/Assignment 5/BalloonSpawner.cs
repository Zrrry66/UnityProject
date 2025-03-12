using UnityEngine;
using Unity.Netcode;

public class BalloonSpawner : NetworkBehaviour 
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
    
    private bool isSpawning = false; //to control the spawning

    private void Awake()
    {
        Instance = this; // let Balloon.cs access spawnRange
    }

    private void Start()
    {
        // Modified: Only start spawning if server
        if (IsServer)
        {
            InvokeRepeating(nameof(SpawnBalloon), 1f, spawnInterval);
            isSpawning = true;
        }
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
        if (!isSpawning || balloonTypes.Length == 0) 
        {
             Debug.Log("Not spawning: either not spawning flag or no balloon types");
            return;
        }
 		if (!IsServer) 
         {
             Debug.Log("Not spawning: not server");
             return; // Modified: only server spawns
         }


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

            GameObject balloonInstance = Instantiate(selectedBalloon, spawnPosition, Quaternion.identity);

             if (selectedBalloon == balloonTypes[3].prefab)
    {
        balloonInstance.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

// Modified: Spawn as network object
            NetworkObject netObj = balloonInstance.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn();
            }
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnBalloon));  //Stop InvokeRepeating
    }

public void StartSpawning()
{
    isSpawning = true;
    CancelInvoke(nameof(SpawnBalloon)); // **防止重复调用**
    InvokeRepeating(nameof(SpawnBalloon), 1f, spawnInterval);
}
}
