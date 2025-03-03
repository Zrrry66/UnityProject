using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float riseSpeed = 2f; // 气球上升速度
    private float maxHeight; // 气球销毁的高度
    private Rigidbody rb; // Rigidbody 组件

    void Start()
    {
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();

        // 确保 Rigidbody 存在并禁用重力
        if (rb != null)
        {
            rb.useGravity = false; // 关闭重力，防止气球掉下来
            rb.velocity = Vector3.up * riseSpeed; // 让气球持续上升
        }

        // 计算最大高度（超过 spawnRange.y 后销毁）
        if (BalloonSpawner.Instance != null)
        {
            maxHeight = BalloonSpawner.Instance.spawnArea.position.y + BalloonSpawner.Instance.spawnRange.y;
        }
        else
        {
            maxHeight = transform.position.y + 5f; // 备用值
        }
    }

    void Update()
    {
        // 确保 Rigidbody 控制气球上升
        if (rb != null)
        {
            rb.velocity = Vector3.up * riseSpeed;
        }

        // 检查是否超出最大高度，销毁气球
        if (transform.position.y > maxHeight)
        {
            Destroy(gameObject);
        }
    }
}
