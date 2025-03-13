using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Balloon : NetworkBehaviour
{
    public float riseSpeed = 2f; // Speed at which the balloon rises
    private float maxHeight; // Maximum height before the balloon is destroyed
    private Rigidbody rb; // Rigidbody component

    public GameObject explosionEffect; // Prefab for explosion effect
    public GameObject scoreTextPrefab; // Prefab for score text
    public int scoreValue = 10; // Score value when balloon is popped

    private Color balloonColor; // Stores the balloon's color

    public AudioClip popSound;
    private AudioSource audioSource; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.up * riseSpeed;
        }

        if (BalloonSpawner.Instance != null)
        {
            maxHeight = BalloonSpawner.Instance.spawnArea.position.y + BalloonSpawner.Instance.spawnRange.y;
        }
        else
        {
            maxHeight = transform.position.y + 5f;
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            balloonColor = renderer.material.color;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.up * riseSpeed;
        }

        if (transform.position.y > maxHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        
        if (!NetworkManager.Singleton.IsServer) return; 

        if (other.CompareTag("Dart") || other.CompareTag("Bullet"))
        {
            Debug.Log("Dart hit the balloon! Executing Explode()");
            Explode();
        }
    }

     private void Explode()
    {
        // Play pop sound (this can remain local)
        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, transform.position);
        }
        
        // Use a ClientRpc to trigger explosion effects on all clients
        ExplodeEffectsClientRpc(transform.position, balloonColor, scoreValue);

        // Update the score on the server side
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogError("ScoreManager Instance not found");
        }

        // Destroy the balloon on the server (this will be replicated to clients)
        Destroy(gameObject);
    }

    [ClientRpc]
    private void ExplodeEffectsClientRpc(Vector3 pos, Color color, int score)
    {
        // Instantiate explosion effect on each client
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, pos, Quaternion.identity);
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = color;
            }
            Destroy(explosion, 2f);
        }
        
        // Instantiate score text on each client
        if (scoreTextPrefab != null)
        {
            GameObject scoreText = Instantiate(scoreTextPrefab, pos, Quaternion.identity);
            TMP_Text textMeshPro = scoreText.GetComponent<TMP_Text>();
            if (textMeshPro != null)
            {
                textMeshPro.text = score.ToString();
            }
            // Adjust the score text position and rotation 
            Vector3 newPosition = textMeshPro.transform.position + new Vector3(0f, 0.6f, 0.2f);
            textMeshPro.transform.position = newPosition;
            textMeshPro.transform.rotation = Quaternion.Euler(0, -90, 0);
            Destroy(scoreText, 2f);
        }
    }
}