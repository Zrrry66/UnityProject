using UnityEngine;

public class TestTrigger : MonoBehaviour
{
     void Start()
    {
   
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TestTrigger detected: " + other.gameObject.name);
    }
    
}