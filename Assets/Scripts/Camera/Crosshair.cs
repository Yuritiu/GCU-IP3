using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //!- Coded By Charlie -!

    [Header("References")]
    [SerializeField] RectTransform crosshairInner;
    [SerializeField] RectTransform crosshairOuter;

    [Header("Variables")]
    [SerializeField] float hoverScale = 1.1f;
    [SerializeField] float tweenDuration = 0.25f;

    Vector3 originalOuterScale;
    Vector3 targetScale;
    float scaleTimer;
    bool isHovering;

    void Start()
    {
        if (crosshairOuter != null)
        {
            originalOuterScale = new Vector3(1, 1, 1);
            targetScale = originalOuterScale;
        }
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "GameScene")
        {
            //Raycast To Mouse Position
            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.transform.CompareTag("Opponent") && GameManager.Instance.canPlay) ||
                    (hit.transform.CompareTag("Card") && GameManager.Instance.canPlay) ||
                    (hit.transform.CompareTag("Rulebook") && GameManager.Instance.canPlay))
                {
                    HoverScale(true);
                }
                else
                {
                    HoverScale(false);
                }
            }
            else
            {
                HoverScale(false);
            }

            scaleTimer += Time.deltaTime / tweenDuration;
            crosshairOuter.localScale = Vector3.Lerp(crosshairOuter.localScale, targetScale, scaleTimer);

            if (!GameManager.Instance.canPlay)
            {
                //Disables Inner Crosshair When Not Players Turn
                HoverScale(false);
                return;
            }

            else if (GameManager.Instance.crosshairUnlocked)
            {

                Vector3 mousePosition = Input.mousePosition;
                crosshairInner.position = new Vector2(mousePosition.x, mousePosition.y);
                crosshairOuter.position = new Vector2(mousePosition.x, mousePosition.y);

            }
            else
            {
                // When the mouse is locked, move the crosshair to the center of the screen
                if (crosshairInner != null && crosshairOuter != null)
                {
                    Vector2 centerPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    crosshairInner.position = centerPosition;
                    crosshairOuter.position = centerPosition;
                }
            }
        }
        else if (currentScene == "MultiplayerGameScene")
        {
            //TODO: FIGURE OUT CARD HOVERING IN MULTIPLAYER
        }
    }

    public void HoverScale(bool hovering)
    {
        isHovering = hovering;

        if (isHovering)
        {
            targetScale = originalOuterScale * hoverScale;
        }
        else
        {
            targetScale = originalOuterScale;
        }
        scaleTimer = 0;
    }
}