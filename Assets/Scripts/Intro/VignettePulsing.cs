using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignettePulsing : MonoBehaviour
{
    public Volume postProcessingVolume;
    public float minSmoothness = 0.2f;
    public float maxSmoothness = 0.5f;
    public float pulseSpeed = 1.0f;

    private Vignette vignette;
    private float targetSmoothness;
    private float currentSmoothness;

    void Start()
    {
        if (postProcessingVolume.profile.TryGet(out vignette))
        {
            currentSmoothness = vignette.smoothness.value;
            targetSmoothness = currentSmoothness;
        }
    }

    void Update()
    {
        float timeFactor = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
        targetSmoothness = Mathf.Lerp(minSmoothness, maxSmoothness, timeFactor);

        currentSmoothness = Mathf.Lerp(currentSmoothness, targetSmoothness, Time.deltaTime * pulseSpeed);
        vignette.smoothness.Override(currentSmoothness);
    }
}
