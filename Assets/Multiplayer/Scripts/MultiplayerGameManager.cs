using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour
{
    public static MultiplayerGameManager Instance;

    [SerializeField] int playerCount;
    PlayerClass[] playerClass;

    bool comparingFinished = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //playerCount = //GETPLAYERCOUNT
        //GET PLAYER CLASSES AT RUNTIME
        //MAKE SURE PLAYERS ARE SET 1 -> PLAYER COUNT
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckPlayerCards();
        }
    }

    void CheckPlayerCards()
    {
        for (int player = 0; player < playerCount - 1; player++)
        {
            for (int cardHand = 0; cardHand < playerCount * 6 - 2; cardHand++)
            {
                if (playerClass[player].hand[cardHand].tag == "cigar")
                {
                    //playerClass[i].hand[j][k].CloneCard();
                    //REWRITE CIGAR LOGIC TO COPY CARD
                    Debug.Log("CIGAR CARD FOUND");
                }
            }
        }

        for (int player = 0; player < playerCount - 1; player++)
        {
            int opponent;
            int opponentHandNumber = 0;

            for (int handNumber = 0; handNumber < playerCount - 2; handNumber++)
            {
                switch (player + handNumber)
                {
                    case 5:
                        opponent = 1;
                        break;

                    case 6:
                        opponent = 2;
                        break;

                    case 7:
                        opponent = 3;
                        break;

                    default:
                        opponent = player + handNumber;
                        break;
                }

                switch (handNumber)
                {
                    case 0:
                        opponentHandNumber = 2;
                        break;

                    case 1:
                        opponentHandNumber = 1;
                        break;

                    case 2:
                        opponentHandNumber = 0;
                        break;
                }

                if (player == playerCount && handNumber == playerCount - 1)
                {
                    comparingFinished = true;
                }

                GameObject[] cardsInUse = new GameObject[2];
                GameObject[] opponentCardsInUse = new GameObject[2];

                switch (handNumber)
                {
                    case 0:
                    cardsInUse[0] = playerClass[player].hand[handNumber + 0];
                    opponentCardsInUse[0] = playerClass[opponent].hand[opponentHandNumber + 0];
                        break;
                    case 1:
                        cardsInUse[1] = playerClass[player].hand[handNumber + 1];
                        opponentCardsInUse[1] = playerClass[opponent].hand[opponentHandNumber + 1];
                        break;
                    case 2:
                        cardsInUse[0] = playerClass[player].hand[handNumber + 0];
                        opponentCardsInUse[0] = playerClass[opponent].hand[opponentHandNumber + 0];
                        break;
                    case 3:
                        cardsInUse[1] = playerClass[player].hand[handNumber + 1];
                        opponentCardsInUse[1] = playerClass[opponent].hand[opponentHandNumber + 1];
                        break;
                    case 4:
                        cardsInUse[0] = playerClass[player].hand[handNumber + 0];
                        opponentCardsInUse[0] = playerClass[opponent].hand[opponentHandNumber + 0];
                        break;
                    case 5:
                        cardsInUse[1] = playerClass[player].hand[handNumber + 1];
                        opponentCardsInUse[1] = playerClass[opponent].hand[opponentHandNumber + 1];
                        break;
                }

                Compare(cardsInUse, opponentCardsInUse, playerClass[player]);
            }
        }
    }

    public void Compare(GameObject[] playerhand, GameObject[] opponenthand, PlayerClass playerClass)
    {
        int oknife = 0;
        int ogun = 0;
        int obottle = 0;

        int parmour = 0;
        int poneinthechamber = 0;
        int pemptypromise = 0;

        for (int i = 0; i < 2; i++)
        {
            //CHANGE STRINGS TO BE CARD TAGS
            switch (playerhand[i].tag)
            {
                case "armour":
                    parmour++;
                    break;
                case "oneInChamber":
                    poneinthechamber++;
                    break;
                case "emptyPromise":
                    pemptypromise++;
                    break;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            //CHANGE STRINGS TO BE CARD TAGS
            switch (opponenthand[i].tag)
            {
                case "knife":
                    oknife++;
                    break;
                case "gun":
                    ogun++;
                    break;
                case "bottle":
                    obottle++;
                    break;
            }
        }
    }
}
