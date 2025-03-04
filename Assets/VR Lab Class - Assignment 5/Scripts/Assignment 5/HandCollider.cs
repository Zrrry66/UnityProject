using Unity.Netcode;
using UnityEngine;

public class HandCollider : MonoBehaviour
{
    #region Member Variables

    public bool isColliding = false;
    public GameObject collidingObject = null;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        if (!GetComponentInParent<NetworkObject>().IsOwner)
        {
            Destroy(this);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log ($"Handcollider collided with {other.gameObject.name}");
        if (!isColliding)
        {
            collidingObject = other.gameObject;
            isColliding = true;
            Debug.Log ($"set collidingobject to: {collidingObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isColliding && other.gameObject == collidingObject)
        {
            collidingObject = null;
            isColliding = false;
        }
    }

    #endregion
}
