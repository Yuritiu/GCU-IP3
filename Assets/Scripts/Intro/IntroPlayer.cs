using System.Collections;
using UnityEngine;

public class IntroPlayer : MonoBehaviour
{
    [Header("Drag Settings")]
    public float initialDelay = 3f;
    public float dragDistance = -1.5f;
    public float dragSpeed = 0.8f;
    public float pauseDuration = 0.8f;

    [Header("Player Control")]
    public bool playerLocked = true;
    public int dragCount = 3;

    [Header("Drag State")]
    private bool dragStarted = false;


    private void Update()
    {
        if (!playerLocked && !dragStarted)
        {
            StartCoroutine(StartWithDelay());
            dragStarted = true;
        }
    }

    IEnumerator StartWithDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(DragLoop());
    }

    IEnumerator DragLoop()
    {
        for (int i = 0; i < dragCount; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + new Vector3(0, 0, dragDistance);
            float totalTime = Mathf.Abs(dragDistance) / dragSpeed;

            float elapsedTime = 0f;
            while (elapsedTime < totalTime)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / totalTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
