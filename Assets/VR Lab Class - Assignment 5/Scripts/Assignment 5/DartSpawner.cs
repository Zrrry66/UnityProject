using UnityEngine;
using Unity.Netcode;

public class DartSpawner : NetworkBehaviour
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
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log($"[DartSpawner] OnNetworkSpawn: spawning dart at {spawnPoint.position}");
            SpawnNewDart();
        }
    }
    
    public void SpawnNewDart()
    {
        if (!IsServer)
        {
            Debug.LogWarning("[DartSpawner] SpawnNewDart called but not on server!");
            return;
        }

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

        Debug.Log($"[{name}] Spawning dart at position {spawnPoint.position}");
        GameObject newDart = Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);

        // Set dart physics status
        Rigidbody rb = newDart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        NetworkObject netObj = newDart.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
            Debug.Log($"[{name}] Dart spawned and network object spawned.");
        }
        else
        {
            Debug.LogWarning($"[{name}] Dart does not have a NetworkObject component.");
        }
    }

    // Client call this to server to spawn new dart
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnNewDartServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[{name}] Received spawn request from client {rpcParams.Receive.SenderClientId}");
        SpawnNewDart();
    }
}
