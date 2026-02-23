using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameSetup : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkObject playerPrefab;

    [Header("Card Manager")]
    [SerializeField] private MultiplayerCardManager cardManager;

    private List<NetworkPlayer> players = new List<NetworkPlayer>();

    public static MultiplayerGameSetup Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; //Only server handles spawning and distributing

        SpawnPlayers();
        DistributeCards();
        StartTurns();
    }

    private void SpawnPlayers()
    {
        players.Clear();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkPlayer np;

            //If auto spawn is off -> manually spawn
            if (client.PlayerObject == null)
            {
                var playerObj = Instantiate(playerPrefab);
                np = playerObj.GetComponent<NetworkPlayer>();
                playerObj.SpawnAsPlayerObject(client.ClientId, true);
            }
            else
            {
                np = client.PlayerObject.GetComponent<NetworkPlayer>();
            }

            players.Add(np);
        }
    }

    private void DistributeCards()
    {
        List<CardType> deck = cardManager.GetShuffledDeck();
        int playerCount = players.Count;
        int cardsPerPlayer = deck.Count / playerCount;

        int index = 0;
        foreach (var np in players)
        {
            List<CardType> hand = deck.GetRange(index, cardsPerPlayer);
            np.PlayerHand.SetCards(hand);
            index += cardsPerPlayer;
        }
    }

    private void StartTurns()
    {
        TurnManager.Instance.InitializePlayers();
        TurnManager.Instance.BeginTurns();
    }
}