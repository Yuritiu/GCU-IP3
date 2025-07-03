using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class CardInfo
{
    public GameObject cardPrefab;
    //6 Knives = 6 Knife Cards Per Each Connected Player
    public int baseQuantity;
    public bool scaleWithPlayerCount = true;
}

[System.Serializable]
public class PlayerHandLayout
{
    //Only For Inspector Reference
    public string playerName;
    public Transform[] slots = new Transform[6];
}


public class InitialCardSpawner : NetworkBehaviour
{
    [SerializeField] public List<CardInfo> cardPrefabs;
    [SerializeField] public Transform deckSpawnPoint;

    [Header("Player Hand Slots")]
    [SerializeField] private List<PlayerHandLayout> predefinedHands;

    private Dictionary<ulong, Transform[]> playerHands = new Dictionary<ulong, Transform[]>();
    private HashSet<ulong> playersReady = new HashSet<ulong>();

    private readonly float[] predefinedZRotations = { 180f, 0f, 90f, 270f };
    public Dictionary<ulong, float> playerZRotations = new Dictionary<ulong, float>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            //Tell Server This Client is Ready Once Scene is Loaded
            StartCoroutine(NotifyReadyAfterDelay());
        }
    }

    IEnumerator NotifyReadyAfterDelay()
    {
        //Wait 1 Sec For Scene to Load Fully
        yield return new WaitForSeconds(1f);
        PlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        playersReady.Add(clientId);

        //Assign Rotation Based on Player Join Order
        int playerIndex = playersReady.Count - 1;
        if (playerIndex < predefinedZRotations.Length)
        {
            playerZRotations[clientId] = predefinedZRotations[playerIndex];
            Debug.Log($"Player {clientId} Card Rotation Set To {playerZRotations[clientId]}");
        }
        else
        {
            //Default Fallback Rotation If More Than 4 Players
            playerZRotations[clientId] = 0f;
        }

        Debug.Log($"Player {clientId} Ready. Total Ready: {playersReady.Count}/{NetworkManager.Singleton.ConnectedClients.Count}");

        if (playersReady.Count == NetworkManager.Singleton.ConnectedClients.Count)
        {
            AssignPlayerHands();
            SpawnAndDealDeck();
        }
    }

    void AssignPlayerHands()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < clients.Count; i++)
        {
            ulong clientId = clients[i].ClientId;

            if (i < predefinedHands.Count)
            {
                playerHands[clientId] = predefinedHands[i].slots;
            }
            else
            {
                Debug.LogError($"No Predefined Card Hand Slots For Player Index {i}");
            }
        }
    }

    void SpawnAndDealDeck()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        List<GameObject> deck = new List<GameObject>();

        foreach (var card in cardPrefabs)
        {
            int totalCount = card.scaleWithPlayerCount ? card.baseQuantity * playerCount : card.baseQuantity;
            for (int i = 0; i < totalCount; i++)
                deck.Add(card.cardPrefab);
        }

        // Shuffle deck
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }

        //Instantiate Deck Stacked Face Down (all cards face down and unowned)
        List<GameObject> deckInstances = new List<GameObject>();
        float yOffset = 0.002f;
        Quaternion faceDownRotation = Quaternion.Euler(90f, 0, 0f);

        for (int i = 0; i < deck.Count; i++)
        {
            Vector3 spawnPos = deckSpawnPoint.position + Vector3.up * (i * yOffset);
            var cardInstance = Instantiate(deck[i], spawnPos, faceDownRotation);

            var netObj = cardInstance.GetComponent<NetworkObject>();
            if (netObj != null)
                //Spawn Without Ownership
                netObj.Spawn();

            deckInstances.Add(cardInstance);
        }

        StartCoroutine(DealCardsRoutine(deckInstances));
    }

    IEnumerator DealCardsRoutine(List<GameObject> deckInstances)
    {
        int playerCount = playerHands.Count;
        int maxCardsPerPlayer = 6;
        int cardsDealt = 0;

        while (cardsDealt < maxCardsPerPlayer * playerCount && cardsDealt < deckInstances.Count)
        {
            foreach (var pair in playerHands)
            {
                ulong clientId = pair.Key;
                Transform[] handSlots = pair.Value;

                int cardIndex = cardsDealt / playerCount;

                if (cardIndex < maxCardsPerPlayer && cardsDealt < deckInstances.Count)
                {
                    GameObject card = deckInstances[cardsDealt];
                    Transform targetSlot = handSlots[cardIndex];

                    var netObj = card.GetComponent<NetworkObject>();
                    var cardVisual = card.GetComponent<NetworkCardVisual>();

                    if (netObj != null && cardVisual != null)
                    {
                        netObj.ChangeOwnership(clientId);

                        if (playerZRotations.TryGetValue(clientId, out float zRot))
                        {
                            cardVisual.ownerZRotation.Value = zRot;
                        }
                        else
                        {
                            cardVisual.ownerZRotation.Value = 0f;
                        }
                    }

                    Quaternion targetRotation = Quaternion.Euler(90f, 0f, 0f);

                    //Lerps Card to Target Slot Face Down
                    yield return StartCoroutine(LerpCardToSlot(card.transform, targetSlot.position, Quaternion.Euler(90f, 0f, 0f), 0.2f));

                    //Flip Face up ONLY on The Owner Client
                    if (netObj != null && netObj.IsOwner)
                    {
                        cardVisual.FlipFaceUp();
                    }

                    //Ask The Client to Flip Their Own Card Using ClientRpc
                    if (netObj != null)
                    {
                        //Target Owner Client to Flip Card
                        FlipCardClientRpc(clientId, netObj.NetworkObjectId, new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        });
                    }

                    cardsDealt++;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    [ClientRpc]
    void FlipCardClientRpc(ulong clientId, ulong cardNetworkObjectId, ClientRpcParams clientRpcParams = default)
    {
        //Only Run on The Client That Owns This Card
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        NetworkObject netObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[cardNetworkObjectId];
        if (netObj != null)
        {
            NetworkCardVisual cardVisual = netObj.GetComponent<NetworkCardVisual>();
            if (cardVisual != null)
            {
                cardVisual.FlipFaceUp();
            }
        }
    }

    IEnumerator LerpCardToSlot(Transform cardTransform, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = cardTransform.position;
        Quaternion startRot = cardTransform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cardTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cardTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cardTransform.position = targetPos;
        cardTransform.rotation = targetRot;
    }
}