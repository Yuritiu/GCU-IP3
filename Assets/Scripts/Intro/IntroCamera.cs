using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensitivityX = 200f;
    public float sensitivityY = 200f;

    [Header("Rotation Limits")]
    public float minXRotation = -90f;
    public float maxXRotation = 90f;
    public float minYRotation = -180f;
    public float maxYRotation = 180f;

    [Header("Camera State")]
    private float xRotation = 0f;
    private float yRotation = 0f;
    public bool cameraLocked = true;

    [Header("Effects Settings")]
    public float dizzyStrength = 10f;
    public float driftSpeed = 1f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (cameraLocked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        float resistanceX = Mathf.Sin(Time.time * 2f) * dizzyStrength * Time.deltaTime;
        float resistanceY = Mathf.Sin(Time.time * 1.5f) * dizzyStrength * Time.deltaTime;

        xRotation -= mouseY - resistanceX;
        yRotation += mouseX - resistanceY;

        xRotation += Mathf.Sin(Time.time * 0.5f) * driftSpeed * Time.deltaTime;
        yRotation += Mathf.Sin(Time.time * 0.3f) * driftSpeed * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        yRotation = Mathf.Clamp(yRotation, minYRotation, maxYRotation);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
