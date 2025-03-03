using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float riseSpeed = 1.5f; // 上升速度

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dart")) // 碰到飞镖销毁
        {
            Destroy(gameObject);
        }
    }
}

