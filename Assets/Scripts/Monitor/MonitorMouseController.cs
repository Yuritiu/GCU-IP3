using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonitorMouseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public RectTransform cursorRect;
    [SerializeField] public RectTransform canvasRect;
    [SerializeField] public Camera uiCamera;

    [SerializeField] public Image cursorImage;

    [Header("Sprites")]
    [SerializeField] public Sprite defaultCursor;
    [SerializeField] public Sprite hoverCursor;

    [Header("Custom Mouse Settings")]
    [SerializeField] public float sensitivityMultiplier = 1850f;
    [SerializeField] public Vector2 minBounds = new Vector2(0f, 0f);
    [SerializeField] public Vector2 maxBounds = new Vector2(400f, 400f);
    private Vector2 cursorPos;

    [Header("Button Variables")]
    private Button currentlyHoveredButton;
    private GameObject currentlyHoveredImage;

    private bool isActive = false;

    public void EnableMouse()
    {
        isActive = true;
        cursorPos = canvasRect.rect.center;
        UpdateCursor();
    }

    public void DisableMouse()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        //On Screen Mouse Movement - Should Be Close Enough To Desktop Mouse Sensitivity
        float mouseX = Input.GetAxis("Mouse X") * sensitivityMultiplier * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityMultiplier * Time.deltaTime;
        cursorPos += new Vector2(mouseX, mouseY);

        ClampCursor();
        UpdateCursor();

        UpdateCursorHover();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClickAtCursor();
        }
    }

    void ClampCursor()
    {
        cursorPos.x = Mathf.Clamp(cursorPos.x, minBounds.x, maxBounds.x);
        cursorPos.y = Mathf.Clamp(cursorPos.y, minBounds.y, maxBounds.y);
    }

    void UpdateCursor()
    {
        cursorRect.anchoredPosition = cursorPos;
    }

    void ClickAtCursor()
    {
        if (currentlyHoveredButton != null)
        {
            //Trigger Clicked Button's Inspector Assigned OnClick Functions
            currentlyHoveredButton.onClick.Invoke();
        }
    }

    void UpdateCursorHover()
    {
        Button hoveredButton = null;

        //2D Box Collider Button Detection
        if (hoveredButton == null)
        {
            Vector2 worldPoint = cursorRect.position;
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            if (hit != null && hit.TryGetComponent(out Button button))
            {
                hoveredButton = button;
            }
        }

        //Debug.Log("Hovered button: " + (hoveredButton ? hoveredButton.name : "None"));

        //Update Hover Visuals
        if (currentlyHoveredButton != hoveredButton)
        {
            if (currentlyHoveredImage != null)
                currentlyHoveredImage.SetActive(false);

            currentlyHoveredButton = hoveredButton;
            currentlyHoveredImage = null;

            if (hoveredButton != null)
            {
                Transform hoverImage = hoveredButton.transform.Find("Hover");
                if (hoverImage != null)
                {
                    currentlyHoveredImage = hoverImage.gameObject;
                    currentlyHoveredImage.SetActive(true);
                }
            }
        }

        cursorImage.sprite = hoveredButton != null ? hoverCursor : defaultCursor;
    }
}