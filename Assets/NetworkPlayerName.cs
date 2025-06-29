using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
using UnityEngine;

public class NetworkPlayerName : NetworkBehaviour
{
    [SerializeField] public TextMeshProUGUI playerListText;

    //NetworkVariable To Sync Serialized Player Names Dictionary
    NetworkVariable<FixedString128Bytes> syncedPlayerNames = new NetworkVariable<FixedString128Bytes>(new FixedString128Bytes(""));

    //Local Deserialized Dictionary clientId -> name
    Dictionary<ulong, string> playerNamesDict = new Dictionary<ulong, string>();

    void Start()
    {
        syncedPlayerNames.OnValueChanged += OnPlayerNamesChanged;
    }

    void OnDestroy()
    {
        syncedPlayerNames.OnValueChanged -= OnPlayerNamesChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            
        }
    }

    //Called By Client To Send Their Name To Server
    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerNameServerRpc(ulong clientId, string playerName)
    {
        if (!playerNamesDict.ContainsKey(clientId))
        {
            playerNamesDict.Add(clientId, playerName);
        }
        else
        {
            playerNamesDict[clientId] = playerName;
        }

        SyncNamesToClients();
    }

    void SyncNamesToClients()
    {
        //Serialize Dictionary To String
        var sb = new StringBuilder();
        foreach (var pair in playerNamesDict)
        {
            sb.Append($"{pair.Key}:{pair.Value}|");
        }
        string serialized = sb.ToString().TrimEnd('|');
        syncedPlayerNames.Value = new FixedString128Bytes(serialized);
    }

    void OnPlayerNamesChanged(FixedString128Bytes oldVal, FixedString128Bytes newVal)
    {
        playerNamesDict = DeserializeNames(newVal);
        UpdatePlayerListUI();
    }

    Dictionary<ulong, string> DeserializeNames(FixedString128Bytes data)
    {
        var dict = new Dictionary<ulong, string>();
        var entries = data.ToString().Split('|');
        foreach (var entry in entries)
        {
            var parts = entry.Split(':');
            if (parts.Length == 2 && ulong.TryParse(parts[0], out ulong clientId))
            {
                dict[clientId] = parts[1];
            }
        }
        return dict;
    }

    void UpdatePlayerListUI()
    {
        var sb = new StringBuilder();
        foreach (var name in playerNamesDict.Values)
        {
            sb.AppendLine(name);
        }
        playerListText.text = sb.ToString();
    }

    //Client Calls This On Local Player To Send Their Name
    public void SubmitLocalPlayerName(string playerName)
    {
        if (IsClient && !IsServer)
        {
            SubmitPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, playerName);
        }
        else if (IsServer)
        {
            //Server Can Set It's Own Name Directly
            playerNamesDict[NetworkManager.Singleton.LocalClientId] = playerName;
            SyncNamesToClients();
        }
    }
}
