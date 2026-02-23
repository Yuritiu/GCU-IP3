using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    [Header("Turn Settings")]
    [SerializeField] private float turnDelay = 1f; //delay between turns
    private int currentPlayerIndex = 0;

    private List<NetworkPlayer> players = new List<NetworkPlayer>();

    public static TurnManager Instance;

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
        if (!IsServer) return; //only server manages turns

        //Collect all network players
        players.Clear();
        foreach (var playerObj in FindObjectsOfType<NetworkPlayer>())
        {
            players.Add(playerObj);
        }

        if (players.Count > 0)
        {
            StartCoroutine(StartTurns());
        }
    }

    public void InitializePlayers()
    {
        players.Clear();
        foreach (var playerObj in FindObjectsOfType<NetworkPlayer>())
        {
            players.Add(playerObj);
        }

        currentPlayerIndex = 0;
    }

    public void BeginTurns()
    {
        if (players.Count == 0) return;
        StartCoroutine(StartTurns());
    }

    private IEnumerator StartTurns()
    {
        while (true)
        {
            if (players.Count == 0)
                yield break;

            NetworkPlayer currentPlayer = players[currentPlayerIndex];

            //Notify the client it's their turn
            currentPlayer.StartTurnClientRpc();

            //Wait until player finishes their turn
            yield return new WaitUntil(() => currentPlayer.IsMyTurn == false);

            // mall delay before next turn
            yield return new WaitForSeconds(turnDelay);

            //Move to next player
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }
    }

    #region Server -> Client RPCs
    [ClientRpc]
    public void StartPlayerTurnClientRpc(ulong clientId)
    {
        foreach (var player in players)
        {
            if (player.OwnerClientId == clientId)
                player.StartTurn();
            else
                player.EndTurn();
        }
    }
    #endregion
}