using Unity.Netcode;
using UnityEngine;

public class DartSpawner : NetworkBehaviour
{
    public GameObject dartPrefab;
    public Transform spawnPoint;  
    private GameObject currentDart;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = this.transform;
        }
    }

    private void Start()
    {
        if (IsServer) // 服务器负责初始化飞镖
        {
            SpawnNewDart();
        }
    }

    [Rpc(SendTo.Server)]
    public void RequestSpawnNewDartRpc()
    {
        if (!IsServer) return;
        SpawnNewDart();
    }

    public void SpawnNewDart()
    {
        if (dartPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning($"[{name}] DartPrefab or spawnPoint not assigned, cannot spawn!");
            return;
        }

        if (currentDart != null)
        {
            Debug.Log($"Dart already exists at {spawnPoint.position}, not spawning a new one.");
            return;
        }

        GameObject newDart = Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkObject netObj = newDart.GetComponent<NetworkObject>();
        if (netObj != null) netObj.Spawn(); // 服务器生成，并同步到所有客户端

        currentDart = newDart;

        Rigidbody rb = newDart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true; 
        }

        Debug.Log($"Dart spawned at {spawnPoint.position}");
    }
}


/* this works. need to change to networkbehaviour
using UnityEngine;

public class DartSpawner : MonoBehaviour
{
    public GameObject dartPrefab;
    public Transform spawnPoint; 

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = this.transform;
        }
    }

    public void SpawnNewDart()
    {
        if (dartPrefab == null)
        {
            Debug.LogWarning($"[{name}] DartPrefab not assigned, cannot spawn!");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[{name}] SpawnPoint is null, cannot spawn!");
            return;
        }

        GameObject newDart = Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);

    // Make the new dart completely still
    Rigidbody rb = newDart.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = true;   // If true, has a small movement. Looks better
    }
    }
}
*/