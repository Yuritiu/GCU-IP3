using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CosmeticPreviewRotator : MonoBehaviour
{
    [Header("Target Rotation Root")]
    [SerializeField] public Transform playerModel;

    [Header("Rotation Settings")]
    [SerializeField] public float rotationSpeed = 20f;

    [Header("UI References")]
    [SerializeField] public GameObject customizationPanel;

    private float currentYRotation = -90f;
    private float baseYRotation = -90f;
    private bool isDragging = false;
    private Vector2 lastMousePosition;

    void Update()
    {
        //Disable Rotation If Hovering UI Panel
        if (IsPointerOverUI(customizationPanel)) return;

        if (isDragging)
        {
            float delta = Input.mousePosition.x - lastMousePosition.x;
            currentYRotation -= delta * Time.deltaTime * rotationSpeed;

            //Clamp to -90 to +90 Degrees From Base Rotation
            float minRotation = baseYRotation - 90f;
            float maxRotation = baseYRotation + 90f;

            currentYRotation = Mathf.Clamp(currentYRotation, minRotation, maxRotation);

            //Apply The Same Rotation to Both Player & Hat
            playerModel.rotation = Quaternion.Euler(0f, currentYRotation, 0f);

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    //public void ResetRotation()
    //{
    //    currentYRotation = -90f;
    //    playerModel.localRotation = Quaternion.identity;
    //    if (hatAttachmentPoint != null)
    //        hatAttachmentPoint.localRotation = Quaternion.identity;
    //}

    public static bool IsPointerOverUI(GameObject targetRootUI)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject == null) continue;

            //If The Hit UI Object Is Part of The Specified Panel
            if (result.gameObject.transform.IsChildOf(targetRootUI.transform))
                return true;
        }

        return false;
    }
}