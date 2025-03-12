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

    // 当这个 NetworkBehaviour 被网络系统正式激活后调用
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log($"[DartSpawner] OnNetworkSpawn: spawning dart at {spawnPoint.position}");
            SpawnNewDart();
        }
    }

    // 仅在服务器上执行 dart 的生成逻辑
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

        // 设置 dart 的物理状态
        Rigidbody rb = newDart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 将 dart 注册为网络对象
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

    // 客户端调用此 RPC 请求服务器生成新的 dart
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnNewDartServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[{name}] Received spawn request from client {rpcParams.Receive.SenderClientId}");
        SpawnNewDart();
    }
}


/*using UnityEngine;
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

    public void SpawnNewDart()
    {
		if (!IsServer) return; // Modified: spawn only on server

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
 
 Debug.Log($"Spawning dart at position: {spawnPoint.position}");

        GameObject newDart = Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);
    // Modified: Spawn as network object
        NetworkObject netObj = newDart.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
             Debug.Log("Dart spawned and network object spawned.");
        }
        else
    {
        Debug.LogWarning("Dart does not have a NetworkObject component.");
    }


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
