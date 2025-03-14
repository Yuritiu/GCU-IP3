using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    public Image blackoutImage;
    public float minFadeTime = 0.5f;
    public float maxFadeTime = 1.5f;
    public int blinkCount = 6;
    public float blinkSpeed = 0.2f;
    public float pauseDuration = 4f;

    private void Start()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        int remainingBlinks = blinkCount;
        bool fadingIn = true;

        while (remainingBlinks > 0)
        {
            float fadeTime = Random.Range(minFadeTime, maxFadeTime);
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = fadingIn ? Mathf.Lerp(0f, 1f, elapsedTime / fadeTime) : Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                blackoutImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }

            fadingIn = !fadingIn;
            if (!fadingIn)
            {
                remainingBlinks--;
                yield return new WaitForSeconds(blinkSpeed);
            }
        }

        yield return new WaitForSeconds(pauseDuration);

        remainingBlinks = blinkCount;
        while (remainingBlinks > 0)
        {
            float fadeTime = Random.Range(minFadeTime, maxFadeTime);
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = fadingIn ? Mathf.Lerp(0f, 1f, elapsedTime / fadeTime) : Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                blackoutImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }

            fadingIn = !fadingIn;
            if (!fadingIn)
            {
                remainingBlinks--;
                yield return new WaitForSeconds(blinkSpeed);
            }
        }

        float finalFadeTime = Random.Range(minFadeTime, maxFadeTime);
        float finalElapsedTime = 0f;
        while (finalElapsedTime < finalFadeTime)
        {
            finalElapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, finalElapsedTime / finalFadeTime);
            blackoutImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}
