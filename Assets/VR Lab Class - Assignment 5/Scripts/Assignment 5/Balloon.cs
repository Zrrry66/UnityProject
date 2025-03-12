using UnityEngine;
using TMPro;

public class Balloon : MonoBehaviour
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
        if (other.CompareTag("Dart") || other.CompareTag("Bullet"))
        {
            Debug.Log("Dart hit the balloon! Executing Explode()");
            Explode();
        }
    }

    private void Explode()
    {
        // Use PlayClipAtPoint to ensure the sound plays even after this object is destroyed.
        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, transform.position);
        }
        
        // Instantiate explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var mainModule = particleSystem.main;
                mainModule.startColor = balloonColor;
            }
            Destroy(explosion, 2f);
        }

        // Update score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogError("ScoreManager Instance not found");
        }

        ShowScore();
        Destroy(gameObject);
    }

    private void ShowScore()
    {
        if (scoreTextPrefab != null)
        {
            GameObject scoreText = Instantiate(scoreTextPrefab, transform.position, Quaternion.identity);
            TMP_Text textMeshPro = scoreText.GetComponent<TMP_Text>();
            if (textMeshPro != null)
            {
                textMeshPro.text = scoreValue.ToString();
            }
            else
            {
                Debug.LogError("TMP_Text component is missing on " + scoreText.name);
            }

            // Adjust the score text position and rotation
            Vector3 newPosition = textMeshPro.transform.position + new Vector3(0f, 0.6f, 0.2f);
            textMeshPro.transform.position = newPosition;
            textMeshPro.transform.rotation = Quaternion.Euler(0, -90, 0);
            Destroy(scoreText, 2f);
        }
    }
}

/*
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float riseSpeed = 2f; // balloon rise speed
    private float maxHeight; // balloon destroy height
    private Rigidbody rb; // Rigidbody component

    void Start()
    {
        // get component
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.up * riseSpeed; // keep ballon rising up
        }

        // calculate max height
        if (BalloonSpawner.Instance != null)
        {
            maxHeight = BalloonSpawner.Instance.spawnArea.position.y + BalloonSpawner.Instance.spawnRange.y;
        }
        else
        {
            maxHeight = transform.position.y + 5f;
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
}
*/