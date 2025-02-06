using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public float sensitivityX = 200f;
    public float sensitivityY = 200f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public bool cameraLocked = true;

    public float minXRotation = -90f;
    public float maxXRotation = 90f;
    public float minYRotation = -180f;
    public float maxYRotation = 180f;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, minYRotation, maxYRotation);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
