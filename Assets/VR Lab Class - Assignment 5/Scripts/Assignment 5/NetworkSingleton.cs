using Unity.Netcode;
using UnityEngine;

public class NetworkSingleton : MonoBehaviour
{
    private static NetworkSingleton instance;

    void Start()
    {}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // **防止 Netcode 在场景切换时被销毁**
        }
        else
        {
            Destroy(gameObject); // **如果已有一个实例，防止重复**
        }
    }
}
