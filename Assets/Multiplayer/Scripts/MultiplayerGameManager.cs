using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager Instance;

    [SerializeField] int playerCount;
    PlayerClass[] playerClass;

    bool comparingFinished = false;

    //DELETE THIS
    [HideInInspector] public bool calledSpace = false;

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        //Only The Server Should Do The Player Class Collection
        if (IsServer)
        {
            Debug.Log("[SERVER] Waiting For All Players To Connect...");
            StartCoroutine(WaitAndInitializePlayers());
        }
    }

    IEnumerator WaitAndInitializePlayers()
    {
        Debug.Log("[SERVER] Waiting for all player objects to be assigned...");

        while (true)
        {
            bool allPlayersReady = true;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null)
                {
                    Debug.LogError($"PlayerObject is NULL For ClientID {client.ClientId}");
                    allPlayersReady = false;
                }
                else
                {
                    Debug.Log($"PlayerObject Assigned For ClientID {client.ClientId}: {client.PlayerObject.name}");
                }
            }

            if (allPlayersReady)
            {
                Debug.Log("[SERVER] All player Objects Assigned");
                break;
            }

            yield return null;
        }

        InitializePlayers();
    }

    void InitializePlayers()
    {
        //Count Connected Players
        playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        playerClass = new PlayerClass[playerCount];

        Debug.Log($"[MULTIPLAYER GAME MANAGER] Connected Players: {playerCount}");

        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            Debug.Log($"[MULTIPLAYER GAME MANAGER] PlayerObject For ClientID {client.ClientId}: {playerObject}");
            //--------------------------------------------------------------------------------------------------------------------------------
            // TODO:
            // SPAWN PLAYER PREFAB USING NETWORK TO SEE IF PLAYER OBJECT IS NO LONGER NULL
            //--------------------------------------------------------------------------------------------------------------------------------

            if (playerObject != null)
            {
                PlayerClass playerClassScript = playerObject.GetComponent<PlayerClass>();

                if (playerClassScript == null)
                {
                    playerClassScript = playerObject.gameObject.AddComponent<PlayerClass>();
                    Debug.Log($"[MULTIPLAYER GAME MANAGER] PlayerClass Script Added To Player {client.ClientId}");
                }

                if (playerClassScript != null)
                {
                    //Assign Unique playerNumber To Each Connected Player
                    int playerNum = index + 1;
                    playerClassScript.playerNumber = playerNum;

                    playerClass[index] = playerClassScript;

                    Debug.Log($"[MULTIPLAYER GAME MANAGER] Assigned Player {client.ClientId} => PlayerNumber: {playerNum}");
                }
                else
                {
                    Debug.LogError($"[MULTIPLAYER GAME MANAGER] Player {client.ClientId} Doesn't Have A PlayerClass Component!");
                }
            }
            else
            {
                Debug.LogError($"[MULTIPLAYER GAME MANAGER] PlayerObject Not Found For ClientID {client.ClientId}");
            }

            index++;
        }

        //foreach (var playerClass in playerClass)
        //{
        //    playerClass.CheckCards();
        //}

        Debug.Log("[MULTIPLAYER GAME MANAGER] Initialized Players");
    }

    public void CheckPlayerCards()
    {
        Debug.Log("[MULTIPLAYER GAME MANAGER] Checking Player Cards...");

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