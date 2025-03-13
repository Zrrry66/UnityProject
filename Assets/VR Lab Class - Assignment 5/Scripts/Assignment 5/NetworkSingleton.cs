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
            DontDestroyOnLoad(gameObject); // **Don't destroy Netcode when changing scene**
        }
        else
        {
            Destroy(gameObject); // **Only keep one in the scene**
        }
    }
}
