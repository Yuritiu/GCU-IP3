using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerClass : NetworkBehaviour
{
    [Header("Spawn Logic")]
    public int playerNumber;

    //2 Cards In Each Hand
    [SerializeField] public GameObject[] card1;
    [SerializeField] public GameObject[] card2;
    [SerializeField] public GameObject[] card3;
    //3 Hands
    [SerializeField] public GameObject[] hand;

    //Offense Cards
    public int knife;
    public int gun;
    public int bottle;
    //Utility Cards
    public int oneinthechamber;
    public int dud;

    public override void OnNetworkSpawn()
    {
        //Runs For The Client
        if (IsOwner)
        {
            
        }
        //Runs For The Server
        if (IsServer)
        {
            //Assign Unique Player Numbers Via The Server
            int index = -1;
            var clients = NetworkManager.Singleton.ConnectedClientsList;

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].ClientId == OwnerClientId)
                {
                    index = i;
                    break;
                }
            }

            playerNumber = index + 1;

            Debug.Log($"[Server] Assigned PlayerNumber: {playerNumber} to ClientID {OwnerClientId}");
        }
    }

    public void Update()
    {
        //-------------------------------------------- Change to check for all players ready -------------------------------------------
        if (Input.GetKeyDown(KeyCode.Space) && !MultiplayerGameManager.Instance.calledSpace)
        {
            MultiplayerGameManager.Instance.calledSpace = true;
            MultiplayerGameManager.Instance.CheckPlayerCards();
        }
    }

    public void CheckCards()
    {
        //Loop Through Placed Cards To Be Played
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    hand[i] = card1[0];
                    break;
                case 1:
                    hand[i] = card1[1];
                    break;
                case 2:
                    hand[i] = card2[0];
                    break;
                case 3:
                    hand[i] = card2[1];
                    break;
                case 4:
                    hand[i] = card3[0];
                    break;
                case 5:
                    hand[i] = card3[1];
                    break;
            }
        }
    }

    public void Action()
    {
        if (gun > 0)
        {
            gun--;
            //move camera to gun
            //gunAction()
            return;
        }


        if (knife > 0)
        {
            knife--;
            //move camera to look at hand
            //knifeAction()
            return;
        }

        if (bottle > 0)
        {
            bottle--;
            //move camera to look at hand
            //BottleAction()
            return;
        }

        if (oneinthechamber > 0)
        {
            oneinthechamber--;
            //move camera to look at hand
            //oneinthechamberAction()
            return;
        }

        if (dud > 0)
        {
            dud--;
            //move camera to look at hand
            //dudAction()
            return;
        }

        knife = 0;
        gun = 0;
        bottle = 0;
        oneinthechamber = 0;
        dud = 0;

        //GameManager.player1Turn = false;
        //GameManager.DoAction();
    }
}