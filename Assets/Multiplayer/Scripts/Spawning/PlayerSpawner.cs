using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;

    bool playersSpawned = false;
    int nextSpawnIndex = 0;

    void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == SceneManager.GetActiveScene().name && NetworkManager.Singleton.IsServer && !playersSpawned)
        {
            Debug.Log($"[PLAYER SPAWNER] Scene {sceneName} Loaded. Spawning Players.");
            SpawnAllPlayers();
            playersSpawned = true;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    void SpawnAllPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log($"[PLAYER SPAWNER] Client Connected: {clientId}. Spawning Player.");
        SpawnPlayer(clientId);
    }

    void SpawnPlayer(ulong clientId)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PLAYER SPAWNER] Player Prefab Not Assigned!");
            return;
        }

        Vector3 spawnPos = GetNextSpawnPosition();

        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        Debug.Log($"[PLAYER SPAWNER] Spawned Player {clientId} at Spawn Point Index {nextSpawnIndex - 1}");
    }

    Vector3 GetNextSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[PLAYER SPAWNER] No Spawn Points Assigned!");
            return Vector3.zero;
        }

        //Loop Through Spawn Points in Order
        Vector3 spawnPosition = spawnPoints[nextSpawnIndex].position;

        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Count;

        return spawnPosition;
    }
}