using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public InputActionProperty shootAction;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No audiosource found on gun! plzz dd one");
        }
    }
    private void Update()
    {
        if (!GameManager.Instance.IsGameRunning()) return; // 游戏未开始时，不允许射击

        Debug.Log("Update running...");

        if (shootAction.action.WasPressedThisFrame())
        {
            Debug.Log("Trigger pressed. Shooting bullet..");
            ShootBullet();
        }
    }

    private void Start()
{
    this.enabled = false; // Enable it when grabbed
}


    private void ShootBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;


        // Spawn bullets
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(90,0,0));
        bullet.SetActive(true);

        Debug.Log("Bullet spawned");
        Debug.Log($"Bullet spawned at {firePoint.position}");
        
        if (audioSource != null)
        {
            audioSource.Play();
        }


        // Add rigidbody
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
             Vector3 shootDirection = firePoint.forward.normalized;
        //Debug.Log($"firePoint.forward: {firePoint.forward} | shootDirection: {shootDirection}");
            
            rb.velocity = shootDirection * bulletSpeed;
            //rb.velocity = firePoint.forward * bulletSpeed;
            //rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            //Debug.Log($"Bullet velocity set to {rb.velocity}");
        }
        else
        {
            Debug.Log("can't find rigidbody");
        }

		// Modified: Spawn bullet as network object
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        // Destroy bullet
        Destroy(bullet, 3f);
    }
}
