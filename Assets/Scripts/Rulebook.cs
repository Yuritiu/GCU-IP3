using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rulebook : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Transform Variables")]
    public Vector3 targetPosition = new Vector3(5, 1, 5);
    public Quaternion targetRotation = Quaternion.Euler(0, 90, 0);
    public float transitionSpeed = 2.0f; 
    public bool inHand = false;

    [Header("Rulebook Pages")]
    public List<GameObject> pages;
    private int currentPage = 0;

    [Header("Enable / Disable")]
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    private IntroTutorial introTutorial;
    private bool stepCompleted = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        for (int i = 1; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }

        introTutorial = FindObjectOfType<IntroTutorial>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Rulebook"))
            {
                if (!inHand)
                {
                    StartCoroutine(MoveRulebook(originalPosition, targetPosition, originalRotation, targetRotation));
                    EnableObjects();
                }
                else
                {
                    StartCoroutine(MoveRulebook(targetPosition, originalPosition, targetRotation, originalRotation));
                    DisableObjects();
                }

                if (introTutorial != null && !stepCompleted)
                {
                    introTutorial.CompleteStep(1);
                    stepCompleted = true;
                }
            }
        }

        if (inHand)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TurnPageForward();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TurnPageBackward();
            }
        }
    }

    private IEnumerator MoveRulebook(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        isMoving = true;
        float transitionProgress = 0.0f;

        while (transitionProgress < 1.0f)
        {
            transitionProgress += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, transitionProgress);
            transform.rotation = Quaternion.Lerp(startRot, endRot, transitionProgress);
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        isMoving = false;

        inHand = !inHand;
    }

    private void EnableObjects()
    {
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    private void DisableObjects()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(false);
        }
    }

    private void TurnPageForward()
    {
        if (currentPage < pages.Count - 1)
        {
            pages[currentPage].SetActive(false); 
            currentPage++; 
            pages[currentPage].SetActive(true); 
        }
    }

    private void TurnPageBackward()
    {
        if (currentPage > 0)
        {
            pages[currentPage].SetActive(false);
            currentPage--; 
            pages[currentPage].SetActive(true); 
        }
    }
}
