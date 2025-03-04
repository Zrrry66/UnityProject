using UnityEngine;

public class DartSpawner : MonoBehaviour
{
    public GameObject dartPrefab; // 预制体
    public Transform spawnPoint; // 生成位置

    public void SpawnNewDart()
    {
        if (dartPrefab != null)
        {
            Instantiate(dartPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}