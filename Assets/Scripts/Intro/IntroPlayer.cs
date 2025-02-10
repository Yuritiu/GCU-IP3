using System.Collections;
using UnityEngine;

public class IntroPlayer : MonoBehaviour
{
    public float dragDistance = -1f;
    public float dragSpeed = 1f;
    public float pauseDuration = 0.4f;
    public bool playerLocked = true;
    public int dragCount = 3;

    private bool dragStarted = false;

    private void Update()
    {
        if (!playerLocked && !dragStarted)
        {
            StartCoroutine(DragLoop());
            dragStarted = true;
        }
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
