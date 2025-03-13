using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class RayCaster : MonoBehaviour
{
    public Transform rayOrigin; // Origin of the ray (RightHand)
    public LineRenderer lineRenderer; // Visual representation of the ray
    public InputActionProperty triggerAction; // Trigger button for activation

    private bool isRayActive = false;

    void Start()
    {
        lineRenderer.enabled = false; // Ensure ray starts OFF
    }

    void Update()
    {
        // Prevent ray activation during gameplay
        if (GameManager.Instance.IsGameRunning()) 
        {
            isRayActive = false;
            lineRenderer.enabled = false;
            return;
        }

        HandleTriggerInput();
        if (isRayActive) HandleRaycast();
    }

    private void HandleTriggerInput()
    {
        if (triggerAction.action.WasPressedThisFrame() && !GameManager.Instance.IsGameRunning()) 
        {
            isRayActive = !isRayActive;
            lineRenderer.enabled = isRayActive;
        }
    }

    private void HandleRaycast()
    {
        if (!isRayActive) return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        lineRenderer.SetPosition(0, rayOrigin.position);
        lineRenderer.SetPosition(1, rayOrigin.position + rayOrigin.forward * 5f);

        // **Check UI Interaction First**
        if (CheckUIRaycast(ray)) return;

        // **Check Physical Object Interaction**
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.TryGetComponent<Button>(out Button button))
            {
                if (triggerAction.action.WasPressedThisFrame())
                {
                    button.onClick.Invoke();
                    Debug.Log("Clicked Physical Button: " + button.name);
                }
            }
        }
    }

    private bool CheckUIRaycast(Ray ray)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Camera.main.WorldToScreenPoint(ray.origin + ray.direction * 2f);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent<Button>(out Button button))
            {
                Debug.Log("Button hovered" + button.name);
                if (triggerAction.action.WasPressedThisFrame()) 
                {
                    button.onClick.Invoke();
                    Debug.Log("UI Button Clicked: " + button.name);
                }
                return true;
            }
        }
        return false;
    }

    public void DeactivateRay()
    {
        isRayActive = false;
        lineRenderer.enabled = false;
    }

     public void ReactivateRay()
    {
        isRayActive = true;
        lineRenderer.enabled = true;
    }
}
