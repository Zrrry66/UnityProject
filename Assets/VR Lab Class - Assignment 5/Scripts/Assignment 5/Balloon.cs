using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float riseSpeed = 2f; // Speed at which the balloon rises
    private float maxHeight; // Maximum height before the balloon is destroyed
    private Rigidbody rb; // Rigidbody component

    public GameObject explosionEffect; // Prefab for explosion effect
    public GameObject scoreTextPrefab; // Prefab for score text
    public int scoreValue = 10; // Score value when balloon is popped

    private Color balloonColor; // Stores the balloon's color

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Disable gravity so the balloon does not fall
            rb.velocity = Vector3.up * riseSpeed; // Set balloon to rise
        }

        // Calculate max height before the balloon disappears
        if (BalloonSpawner.Instance != null)
        {
            maxHeight = BalloonSpawner.Instance.spawnArea.position.y + BalloonSpawner.Instance.spawnRange.y;
        }
        else
        {
            maxHeight = transform.position.y + 5f; // Default fallback height
        }

        // Retrieve the balloon's color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            balloonColor = renderer.material.color;
        }
    }

    void Update()
    {
        // Ensure the balloon keeps rising
        if (rb != null)
        {
            rb.velocity = Vector3.up * riseSpeed;
        }

        // Destroy the balloon if it exceeds the max height
        if (transform.position.y > maxHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with" + other.gameObject.name);
        // Check if the balloon was hit by a dart
        if (other.CompareTag("Dart"))
        {
            Debug.Log("Dart hit the balloon! Excuting Explode()");
            Explode();
        }
    }

    private void Explode()
    {
        // Instantiate explosion effect at balloon's position
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Retrieve the Particle System and set its color to match the balloon's color
            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var mainModule = particleSystem.main;
                mainModule.startColor = balloonColor;
            }
        }

        // Display the score
        ShowScore();

        // Destroy the balloon object
        Destroy(gameObject);
    }

    private void ShowScore()
    {
        if (scoreTextPrefab != null)
        {
            // Instantiate the score text at the balloon's position
            GameObject scoreText = Instantiate(scoreTextPrefab, transform.position, Quaternion.identity);
            scoreText.GetComponent<TextMesh>().text = scoreValue.ToString();

            // Destroy the score text after 2 seconds
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