using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    //!- Coded By Charlie -!

    public static TutorialManager Instance;

    [Header("Camera Variables")]
    [SerializeField] GameObject camera;
    [SerializeField] Freelook freelook;

    [Header("Camera Positions")]
    [SerializeField] GameObject pos1;
    //[SerializeField] GameObject pos2;
    //[SerializeField] GameObject pos3;

    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] TutorialCardDraw tutorialCardDraw;

    [Header("Text Variables")]
    [SerializeField] TextMeshProUGUI tutorialText;

    [HideInInspector] public int tutorialPhase = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.isTutorial = true;

        tutorialPhase = -1;

        freelook.canLook = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        //Look At Pos 1
        camera.transform.LookAt(pos1.transform);

        StartCoroutine(FirstPause());
    }

    IEnumerator FirstPause()
    {
        yield return new WaitForSeconds(2);
        //Pos 1 Text
        tutorialText.text = "Hello, welcome to *INSERT NAME*, why don't you start by having a look at your hand in the bottom right.";

        yield return new WaitForSeconds(1);
        freelook.canLook = true;

        yield return new WaitForSeconds(5);
        tutorialText.text = "Now why don't we get started...place a knife and an armour card on the table.";
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;

        tutorialPhase = 0;
    }

    void FixedUpdate()
    {
        //Once The Knife And Armour Have Been Placed, Advance
        if (tutorialPhase == 0)
        {
            if (tutorialCardDraw.selectedPosition1 != null && tutorialCardDraw.selectedPosition2 != null)
            {
                if (tutorialCardDraw.selectedPosition1.GetComponentInChildren<Knife>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Armour>() != null)
                {
                    //Allow Player To Progress
                    tutorialPhase = 1;
                    StartCoroutine(SecondPause());
                }
            }
        }
    }

    IEnumerator SecondPause()
    {
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        yield return new WaitForSeconds(0.1f);

        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;

        tutorialText.text = "Great, the knife will attempt to cut off one finger, and the armour will attempt to stop an attacking knife, or bullet, " +
        "from cutting your finger.";

        yield return new WaitForSeconds(10);
        tutorialText.text = "Now play your hand by clicking the opponent.";

        //Allow Player To Play Hand
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;
    }



}
