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
// Modified: Spawn as network object
        NetworkObject netObj = newDart.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
    }
}