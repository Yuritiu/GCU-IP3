using UnityEngine;

public class Freelook : MonoBehaviour
{
    [Header("Sensitivity Variables")]
    [SerializeField] float mouseSensitivity = 135f;
    [SerializeField] float mouseSmoothing = 0.1f;

    [Header("Clamp Vertical Variables")]
    [SerializeField] float minX = -35f;
    [SerializeField] float maxX = 35f;

    [Header("Clamp Horizontal Variables")]
    [SerializeField] float minY = -75f;
    [SerializeField] float maxY = 75f;

    float xRotation = 0f;
    float currentXRotation = 0f;
    float yRotation = 0f;
    float currentYRotation = 0f;

    [HideInInspector] public bool canLook = true;

    void Start()
    {
        //Hide The Cursor On Game Start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //TODO: ADD CUSTOM CURSOR
        //Cursor.SetCursor()
    }

    void Update()
    {
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
}
