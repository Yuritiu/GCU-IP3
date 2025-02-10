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

    [Header("Rotation and Stairs")]
    public float rotationSpeed = 0.5f;
    public float stairHeight = 0.5f;
    public float stairDepth = 0.5f;
    public float stairDelay = 0.5f;

    public IntroCamera introCamera;

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

        StartCoroutine(WalkDownStairs());
    }
    IEnumerator WalkDownStairs()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + new Vector3(0, -stairHeight, -stairDepth);
            float stepTime = 0.2f;

            float elapsedTime = 0f;
            while (elapsedTime < stepTime)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / stepTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
            yield return new WaitForSeconds(stairDelay);
        }
    }

}
