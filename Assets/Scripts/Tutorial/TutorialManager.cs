using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

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
    [HideInInspector] public bool playedHand = false;
    [HideInInspector] public bool canSkipTurn = false;


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
        yield return new WaitForSeconds(1);
        //Pos 1 Text
        tutorialText.text = "Hello, welcome to *INSERT NAME*, why don't you start by having a look at your hand in the bottom right.";

        yield return new WaitForSeconds(1);
        freelook.canLook = true;

        yield return new WaitForSeconds(3);
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
                if ((tutorialCardDraw.selectedPosition1.GetComponentInChildren<Knife>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Armour>() != null) || (tutorialCardDraw.selectedPosition1.GetComponentInChildren<Armour>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Knife>() != null))
                {
                    //Allow Player To Progress
                    tutorialPhase = 1;
                    StartCoroutine(SecondPause());
                }
            }
        }
        //Advance
        else if (tutorialPhase == 1)
        {
            if (playedHand)
            {
                playedHand = false;
                StartCoroutine(ThirdPause());
            }
        }
        //Once The 2 Knifes Have Been Placed, Advance
        else if (tutorialPhase == 2)
        {
            if (tutorialCardDraw.selectedPosition1 != null && tutorialCardDraw.selectedPosition2 != null)
            {
                if (tutorialCardDraw.selectedPosition1.GetComponentInChildren<Knife>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Knife>() != null)
                {
                    TutorialCardDraw.Instance.canSelectCards = false;

                    canSkipTurn = true;
                    gameManager.canPlay = true;
                    tutorialCardDraw.canPlay = true;
                    tutorialPhase = 3;
                }
            }
        }
        //Advance
        else if (tutorialPhase == 3)
        {
            if (playedHand)
            {
                playedHand = false;
                StartCoroutine(FourthPause());
            }
        }
        //Once The Knife & Skip Turn Have Been Placed, Advance
        else if (tutorialPhase == 4)
        {
            if (tutorialCardDraw.selectedPosition1 != null && tutorialCardDraw.selectedPosition2 != null)
            {
                if ((tutorialCardDraw.selectedPosition1.GetComponentInChildren<Knife>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<SwingAwayCard>() != null) || (tutorialCardDraw.selectedPosition1.GetComponentInChildren<SwingAwayCard>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Knife>() != null))
                {
                    Debug.Log("knife + bat placed");
                    TutorialCardDraw.Instance.canSelectCards = false;

                    canSkipTurn = true;
                    gameManager.canPlay = true;
                    tutorialCardDraw.canPlay = true;
                    tutorialPhase = 5;
                }
            }
        }
        //Advance
        else if (tutorialPhase == 5)
        {
            if (playedHand)
            {
                playedHand = false;
                StartCoroutine(FifthPause());
            }
        }
        //Skip Turn, Advance
        else if (tutorialPhase == 6)
        {
            TutorialCardDraw.Instance.canSelectCards = false;

            canSkipTurn = true;
            gameManager.canPlay = true;
            tutorialCardDraw.canPlay = true;
            tutorialPhase = 7;
        }
        //Once Skipped Advance
        else if (tutorialPhase == 7)
        {
            if (playedHand)
            {
                playedHand = false;
                StartCoroutine(SixthPause());
            }
        }
        //Once The Knife And Cigar Have Been Placed, Advance
        else if (tutorialPhase == 8)
        {
            if (tutorialCardDraw.selectedPosition1 != null && tutorialCardDraw.selectedPosition2 != null)
            {
                if ((tutorialCardDraw.selectedPosition1.GetComponentInChildren<Knife>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<CigarCard>() != null) || (tutorialCardDraw.selectedPosition1.GetComponentInChildren<CigarCard>() != null && tutorialCardDraw.selectedPosition2.GetComponentInChildren<Knife>() != null))
                {
                    TutorialCardDraw.Instance.canSelectCards = false;

                    canSkipTurn = true;
                    gameManager.canPlay = true;
                    tutorialCardDraw.canPlay = true;
                    tutorialPhase = 9;
                }
            }
        }
        //Advance To End
        else if (tutorialPhase == 9)
        {
            if (playedHand)
            {
                playedHand = false;
                StartCoroutine(SeventhPause());
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

        yield return new WaitForSeconds(8);
        tutorialText.text = "Now play your hand by clicking on the opponent.";

        //Allow Player To Play Hand
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;
        canSkipTurn = true;
    }

    IEnumerator ThirdPause()
    {
        canSkipTurn = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        yield return new WaitForSeconds(0.1f);

        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;

        tutorialText.text = "Good, after each turn each player will draw 1 card.";

        yield return new WaitForSeconds(5);
        tutorialText.text = "Now let's play 2 knife cards.";

        TutorialCardDraw.Instance.canSelectCards = true;
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;

        tutorialPhase = 2;
    }

    IEnumerator FourthPause()
    {
        canSkipTurn = false;
        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        //Some weird stuff happens here with timing, no clue why, ig coroutine running cancels any call to NextTurn();?
        yield return new WaitForSeconds(0.1f);

        tutorialText.text = "A skip turn card will skip the opponent's next turn";

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Let's place a skip turn card with a knife";

        //Allow Player To Play Hand
        TutorialCardDraw.Instance.canSelectCards = true;
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;

        tutorialPhase = 4;
    }

    IEnumerator FifthPause()
    {
        Debug.Log("5th Pause");
        canSkipTurn = false;
        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        yield return new WaitForSeconds(0.1f);

        tutorialText.text = "Alright, now that we only have 1 card, lets skip our turn so we can play 2 and combine them next turn.";

        yield return new WaitForSeconds(5f);
        tutorialText.text = "Click the opponent while the table is empty to skip your turn.";

        //Allow Player To Play Hand
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;
        canSkipTurn = true;

        tutorialPhase = 7;
    }

    IEnumerator SixthPause()
    {
        canSkipTurn = false;
        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        yield return new WaitForSeconds(0.1f);

        tutorialText.text = "A cigar card will clone any card it is placed with";

        yield return new WaitForSeconds(4f);

        tutorialText.text = "Now lets use a cigar card with our knife to duplicate the effect";

        //Allow Player To Play Hand
        TutorialCardDraw.Instance.canSelectCards = true;
        gameManager.canPlay = true;
        tutorialCardDraw.canPlay = true;

        tutorialPhase = 8;
    }

    IEnumerator SeventhPause()
    {
        //Stop Player Being Able To Select Remaining Cards From Hand
        TutorialCardDraw.Instance.canSelectCards = false;
        gameManager.canPlay = false;
        tutorialCardDraw.canPlay = false;

        yield return new WaitForSeconds(5f);

        tutorialText.text = "Looks like you get to live, for now at least";

        yield return new WaitForSeconds(4f);

        tutorialText.text = "There are some things that you must figure out on your own though";

        yield return new WaitForSeconds(4f);

        tutorialText.text = "You won't get to keep your blood in the future";

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene("Main Menu");
    }
}
