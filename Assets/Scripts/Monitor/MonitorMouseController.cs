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

        UpdateCursorSprite();

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
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = uiCamera.WorldToScreenPoint(cursorRect.position)
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        foreach (var result in results)
        {
            ExecuteEvents.Execute(result.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }
    }

    void UpdateCursorSprite()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = uiCamera.WorldToScreenPoint(cursorRect.position)
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        bool hoveringButton = false;

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>())
            {
                hoveringButton = true;
                break;
            }
        }

        cursorImage.sprite = hoveringButton ? hoverCursor : defaultCursor;
    }
}