using UnityEngine;

public class DartSpawner : MonoBehaviour
{
    public GameObject dartPrefab;
    public Transform spawnPoint;

    public void SpawnNewDart()
    {
        if (dartPrefab != null)
        {
            Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}