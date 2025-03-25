using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackfireGlow : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    [Range(0, 10)]
    public float duration = 1f;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    
    public void GlowActive()
    {
        StartCoroutine(BackfireShow());
    }

    private IEnumerator BackfireShow()
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float timer = timeElapsed / duration;
            rend.material.color = Color.Lerp(startColor, endColor, timer);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        
        rend.material.color = endColor;
    }
}
