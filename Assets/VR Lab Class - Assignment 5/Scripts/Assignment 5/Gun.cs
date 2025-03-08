using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public InputActionProperty shootAction;

    private void Update()
    {
        Debug.Log("Update running...");

        if (shootAction.action.WasPressedThisFrame())
        {
            Debug.Log("Trigger pressed. Shooting bullet..");
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Spawn bullets
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log("Bullet spawned");
        Debug.Log($"Bullet spawned at {firePoint.position}");
        
        
        // Add rigidbody
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
             Vector3 shootDirection = firePoint.forward.normalized;
        Debug.Log($"firePoint.forward: {firePoint.forward} | shootDirection: {shootDirection}");
            
            rb.velocity = shootDirection * bulletSpeed;
            //rb.velocity = firePoint.forward * bulletSpeed;
            //rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            Debug.Log($"Bullet velocity set to {rb.velocity}");
        }
        else
        {
            Debug.Log("can't find rigidbody");
        }

        // Destroy bullet
        Destroy(bullet, 3f);
    }
}
