using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab; // 预制子弹
    public Transform firePoint; // 子弹发射点
    public float bulletSpeed = 20f; // 子弹速度

    public InputActionProperty shootAction; // 触发器按钮输入

    private void Update()
    {
        if (shootAction.action.WasPressedThisFrame())
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 生成子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // 添加刚体并施加力
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        // 可选：销毁子弹，防止无限堆积
        Destroy(bullet, 3f);
    }
}
