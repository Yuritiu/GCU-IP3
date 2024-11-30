using UnityEngine;
using System.Collections;

public class Freelook : MonoBehaviour
{
    [Header("Sensitivity Variables")]
    [SerializeField] public float mouseSensitivity = 135f;
    [SerializeField] float mouseSmoothing = 0.1f;

    [Header("Clamp Vertical Variables")]
    public float minX = -35f;
    public float maxX = 35f;

    [Header("Clamp Horizontal Variables")]
    public float minY = -75f;
    public float maxY = 75f;

    public float xRotation = 0f;
    public float currentXRotation = 0f;
    public float yRotation = 0f;
    public float currentYRotation = 0f;

    [HideInInspector] public bool canLook = true;
    [HideInInspector] public bool reset;

    void Start()
    {
        //Hide The Cursor On Game Start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameManager.Instance.inBatAction)
        {
            currentXRotation = 0;
            currentYRotation = 0;
            xRotation = 0;
            yRotation = 0;
            canLook = false;
        }
        else if (GameManager.Instance.in2ndPos == true)
        {
            canLook = false;
            xRotation = GameManager.Instance.Target5.position.x;
            yRotation = GameManager.Instance.Target5.position.y;
        }
        else if (GameManager.Instance.in3rdPos == true)
        {
            canLook = false;
            xRotation = GameManager.Instance.Target6.position.x;
            yRotation = GameManager.Instance.Target6.position.y;
        }
        else if (GameManager.Instance.in2ndPos == false && GameManager.Instance.in3rdPos == false && !GameManager.Instance.inBatAction)
        {
            canLook = true;
        }

        if (!canLook || SettingsMenu.Instance.settingsMenuOpen)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Adjust X (Vertical) Rotation And Clamp
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minX, maxX);

        //Adjust Y (Horizontal) Rotation And Clamp
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, minY, maxY);

        //Apply Mouse Smoothing To The Camera To Stop Jittering
        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, mouseSmoothing * Time.deltaTime);
        currentYRotation = Mathf.Lerp(currentYRotation, yRotation, mouseSmoothing * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
    }

    public void SetXRotationSmooth(float targetXRotation, float smoothSpeed)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothXRotation(targetXRotation, smoothSpeed));
    }


    private IEnumerator SmoothXRotation(float targetXRotation, float smoothSpeed)
    {
        while (Mathf.Abs(currentXRotation - targetXRotation) > 0.01f)
        {
            currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
            yield return null;
        }
        currentXRotation = targetXRotation;
        transform.localRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
    }

    public void SetXRotation(float xRotation, bool smooth = false, float smoothSpeed = .5f)
    {
        if (smooth)
        {
            SetXRotationSmooth(xRotation, smoothSpeed);
        }
        else
        {
            currentXRotation = xRotation;
            transform.localRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
        }
    }
}
