using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovLerp : MonoBehaviour
{
    [Header("FOV Settings")]
    [SerializeField] private float targetFovOffset = -5f;
    [SerializeField] private float durationInSeconds = 2f;

    [Header("Internal FOV Variables")]
    [SerializeField] private Camera camera;
    [SerializeField] private float targetFov;
    [SerializeField] private float originalFov;
    [SerializeField] private float timeElapsed;
    [SerializeField] private bool lerping;
    [SerializeField] private bool lerpingBack;

    private GunCameraShake gunCameraShake;

    void Start()
    {
        camera = GetComponent<Camera>();
        originalFov = PlayerPrefs.GetFloat("FOV", 60f);
        targetFov = originalFov + targetFovOffset;
        timeElapsed = 0f;
        lerping = false;
        lerpingBack = false;
    }

    void Update()
    {
        if (lerping)
        {
            timeElapsed += Time.deltaTime;
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, timeElapsed / durationInSeconds);
        }
        else if (lerpingBack)
        {
            timeElapsed += Time.deltaTime;
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, originalFov, timeElapsed / durationInSeconds);

            if (camera.fieldOfView == originalFov)
            {
                lerpingBack = false;
            }
        }

        originalFov = PlayerPrefs.GetFloat("FOV", 50f);
        targetFov = originalFov + targetFovOffset;
    }

    public void StartLerp()
    {
        if (!lerping)
        {
            lerping = true;
            lerpingBack = false;
            timeElapsed = 0f;
        }
    }

    public void StopLerp()
    {
        if (!lerpingBack)
        {
            lerpingBack = true;
            lerping = false;
            timeElapsed = 0f;
        }
    }
}
