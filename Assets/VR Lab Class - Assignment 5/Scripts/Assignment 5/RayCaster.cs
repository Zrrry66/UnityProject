using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class RayCaster : MonoBehaviour
{
    public Transform rayOrigin; // 负责发射射线的起点 (RightHand)
    public LineRenderer lineRenderer; // 射线可视化
    public InputActionProperty triggerAction; // 绑定 Trigger 输入（使用 Activate）

    private bool isRayActive = false;

    void Update()
    {
        HandleTriggerInput();
        if (isRayActive) HandleRaycast();
    }

    private void HandleTriggerInput()
    {
        if (triggerAction.action.WasPressedThisFrame())
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

        // **检查 UI 交互**
        if (CheckUIRaycast()) return;
    }

    private bool CheckUIRaycast()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Screen.width / 2, Screen.height / 2); // 屏幕中心模拟点击位置

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent<Button>(out Button button))
            {
                if (triggerAction.action.WasPressedThisFrame()) // 现在 `Activate` 触发 UI 按钮点击
                {
                    button.onClick.Invoke();
                    Debug.Log("UI Button Clicked: " + button.name);
                }
                return true;
            }
        }
        return false;
    }
}

/*using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class RayCaster : MonoBehaviour
{
    public Transform rayOrigin; // The origin point for the ray (usually the RightHand)
    public LineRenderer lineRenderer; // Visual representation of the ray
    public InputActionProperty triggerAction; // Input action for Trigger (Activate)

    private bool isRayActive = false; // Controls ray visibility

    void Update()
    {
        HandleTriggerInput(); // Handle Trigger button interactions
        if (isRayActive) HandleRaycast(); // Process raycasting only when the ray is active
    }

    private void HandleTriggerInput()
    {
        // When Trigger (Activate action) is pressed, toggle the ray visibility
        if (triggerAction.action.WasPressedThisFrame())
        {
            isRayActive = !isRayActive;
            lineRenderer.enabled = isRayActive;
        }
    }

    private void HandleRaycast()
    {
        if (!isRayActive) return; // If the ray is not active, do nothing

        RaycastHit hit;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        // Draw the ray using LineRenderer
        lineRenderer.SetPosition(0, rayOrigin.position);
        lineRenderer.SetPosition(1, rayOrigin.position + rayOrigin.forward * 5f);

        // **First, check UI interactions**
        if (CheckUIRaycast()) return;

        // **If no UI was hit, check for physical objects**
        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (hit.collider.TryGetComponent<Button>(out Button button))
            {
                if (triggerAction.action.WasPressedThisFrame()) // Use `Activate` to trigger button click
                {
                    button.onClick.Invoke();
                    Debug.Log("Clicked Button: " + button.name);
                }
            }
        }
    }

    // **Checks for UI button interactions**
    private bool CheckUIRaycast()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Screen.width / 2, Screen.height / 2); // Simulating raycasting at the center of the screen

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent<Button>(out Button button))
            {
                if (triggerAction.action.WasPressedThisFrame()) // Use `Activate` to trigger UI button click
                {
                    button.onClick.Invoke();
                    Debug.Log("UI Button Clicked: " + button.name);
                }
                return true;
            }
        }
        return false;
    }
}
*/