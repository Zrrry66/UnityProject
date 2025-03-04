using UnityEngine;

public class DartThrow : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private bool isGrabbed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isGrabbed)
        {
            // 计算速度
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }
    }

    public void OnGrabbed()
    {
        isGrabbed = true;
        rb.isKinematic = true; // 关闭物理模拟
    }

    public void OnReleased(Transform handTransform)
    {
        isGrabbed = false;
        rb.isKinematic = false; // 开启物理模拟

        Vector3 throwDirection = transform.forward;

        rb.velocity = throwDirection * velocity.magnitude * 2.5f; // 根据速度计算抛出速度
        rb.angularVelocity = Vector3.zero; // keep from rotating

        // 额外向前施加力，确保飞镖飞出
        rb.AddForce(handTransform.forward * 2.0f, ForceMode.Impulse);

        rb.AddTorque(Vector3.Cross(transform.forward, rb.velocity).normalized * 10f, ForceMode.Impulse);
    }
}