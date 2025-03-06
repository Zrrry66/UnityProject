using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualHand : MonoBehaviour
{
    #region Enum

    private enum VirtualHandMode
    {
        Snap,
        Reparenting,
        NoReparenting
    }

    #endregion
    
    #region Member Variables

    public InputActionProperty toggleModeAction;
    [SerializeField] private VirtualHandMode virtualHandMode = VirtualHandMode.Snap;

    public InputActionProperty grabAction;
    public HandCollider handCollider;

    private GameObject grabbedObject;
    private Matrix4x4 offsetMatrix;

    /*private bool canGrab
    {
        get
        {
            if (handCollider.isColliding)
                return handCollider.collidingObject.GetComponent<ObjectAccessHandler>().RequestAccess();
            return false;
        }
    }
    */

    private bool canGrab
{
    get
    {
        if (handCollider == null)
        {
            Debug.LogError("handCollider is null in canGrab!");
            return false;
        }

        if (!handCollider.isColliding)
        {
            Debug.Log("canGrab = false: handCollider is not colliding with anything.");
            return false;
        }

        if (handCollider.collidingObject == null)
        {
            Debug.Log("canGrab = false: collidingObject is null.");
            return false;
        }

        ObjectAccessHandler accessHandler = handCollider.collidingObject.GetComponent<ObjectAccessHandler>();
        if (accessHandler == null)
        {
            Debug.LogError($"canGrab = false: Object {handCollider.collidingObject.name} is missing ObjectAccessHandler!");
            return false;
        }

        bool accessGranted = accessHandler.RequestAccess();

        return accessGranted;
    }
}

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        bool isOwner = GetComponentInParent<NetworkObject>().IsOwner;
    Debug.Log($"VirtualHand Start - IsOwner: {isOwner}");

        if (!isOwner)
        {
            Destroy(this);
            return;
        }
    }

    private void Update()
    {
        if (grabAction.action.WasPressedThisFrame())
        Debug.Log("Grab button is pressed.");

        if (toggleModeAction.action.WasPressedThisFrame())
            virtualHandMode = (VirtualHandMode)(((int)virtualHandMode + 1) % 3);

        switch (virtualHandMode)
        {
            case VirtualHandMode.Snap:
                SnapGrab();
                break;
            case VirtualHandMode.Reparenting:
                ReparentingGrab();
                break;
            case VirtualHandMode.NoReparenting:
                CalculationGrab();
                break;
        }
    }

    #endregion

    #region Custom Methods

    /*private void SnapGrab()
    {
        if (grabAction.action.IsPressed())
        {
            Debug.Log($"Grab button was pressed");
            Debug.Log($"Grab action pressed. canGrab: {canGrab}");
            if (grabbedObject == null && canGrab)
            {
                grabbedObject = handCollider.collidingObject;
            }

            if (grabbedObject != null)
            {
                grabbedObject.transform.position = transform.position;
                grabbedObject.transform.rotation = transform.rotation;
            }
        }
        else if (grabAction.action.WasReleasedThisFrame())
        {
            if(grabbedObject != null)
          Debug.Log($"Releasing object:{grabbedObject?.name}");
                grabbedObject.GetComponent<ObjectAccessHandler>().Release();

            grabbedObject = null;
        }

        Debug.Log($"Grabbing object:{grabbedObject?.name}");
    }
    */
   private void SnapGrab()
{
    if (grabAction.action.IsPressed())
    {
        if (grabbedObject == null && canGrab)
        {
            grabbedObject = handCollider.collidingObject;
            
            // Spawn new Dart
            DartSpawner spawner = FindObjectOfType<DartSpawner>();
            if (spawner != null && grabbedObject.CompareTag("Dart"))
            {
                spawner.SpawnNewDart();
            }
        }

        if (grabbedObject != null)
        {
            //grabbedObject.transform.position = transform.position;
            Vector3 offset = new Vector3(-0.025f, 0, 0); // Example offset (adjust as needed)
            grabbedObject.transform.position = transform.position + transform.rotation * offset;

            Quaternion offsetRotation = Quaternion.Euler(0,180,0);
            grabbedObject.transform.rotation = transform.rotation * offsetRotation;
        }
    }
    else if (grabAction.action.WasReleasedThisFrame())
    {
        if (grabbedObject != null)
        {
            var dartThrow = grabbedObject.GetComponent<DartThrow>();
            if (dartThrow != null)
            {
                dartThrow.OnReleased(transform);
            }

            grabbedObject.GetComponent<ObjectAccessHandler>().Release();
        }

        grabbedObject = null;
    }
}

    private void ReparentingGrab()
    {
        if (grabAction.action.WasPressedThisFrame())
        {
            if (grabbedObject == null && canGrab)
            {
                grabbedObject = handCollider.collidingObject;
                grabbedObject.transform.SetParent(transform, true);
            }
        }
        else if (grabAction.action.WasReleasedThisFrame())
        {
            if (grabbedObject != null)
            {
 
                grabbedObject.GetComponent<ObjectAccessHandler>().Release();
                grabbedObject.transform.SetParent(null, true);
            }

            grabbedObject = null;
        }
    }

    private void CalculationGrab()
    {
        if (grabAction.action.WasPressedThisFrame())
        {
            if (grabbedObject == null && canGrab)
            {
                grabbedObject = handCollider.collidingObject;
                offsetMatrix = GetTransformationMatrix(transform, true).inverse *
                               GetTransformationMatrix(grabbedObject.transform, true);
            }
        }
        else if (grabAction.action.IsPressed())
        {
            if (grabbedObject != null)
            {
                Matrix4x4 newTransform = GetTransformationMatrix(transform, true) * offsetMatrix;

                grabbedObject.transform.position = newTransform.GetColumn(3);
                grabbedObject.transform.rotation = newTransform.rotation;
            }
        }
        else if (grabAction.action.WasReleasedThisFrame())
        {
            if(grabbedObject != null)
                grabbedObject.GetComponent<ObjectAccessHandler>().Release();
            grabbedObject = null;
            offsetMatrix = Matrix4x4.identity;
        }
    }

    #endregion
    
    #region Utility Functions

    public Matrix4x4 GetTransformationMatrix(Transform t, bool inWorldSpace = true)
    {
        if (inWorldSpace)
        {
            return Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
        }
        else
        {
            return Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }
    }

    #endregion
}
