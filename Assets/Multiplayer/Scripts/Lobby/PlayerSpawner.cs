using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] public GameObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
            }
        }
    }

    void SpawnPlayer(ulong clientId)
    {
        var playerObject = Instantiate(playerPrefab);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
