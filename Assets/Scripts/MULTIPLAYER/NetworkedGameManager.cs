using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkedGameManager : NetworkBehaviour
{
    public static NetworkedGameManager Instance;

    public List<NetworkPlayer> Players = new List<NetworkPlayer>();
    public GameObject BottlePrefab;

    public float BaseWaitTime = 4.2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //Initialize players
            Players.AddRange(FindObjectsOfType<NetworkPlayer>());
        }
    }

    #region Card Actions
    public void PlayCard(NetworkPlayer source, ICardAction card, NetworkPlayer target)
    {
        if (!IsServer) return;

        card.PlayCardForPlayer(source);
        ApplyCardEffectsServerRpc(source.PlayerId, target.PlayerId, card.CardType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyCardEffectsServerRpc(int sourceId, int targetId, CardType type)
    {
        NetworkPlayer source = Players.Find(p => p.PlayerId == sourceId);
        NetworkPlayer target = Players.Find(p => p.PlayerId == targetId);

        float roll = Random.Range(0f, 100f);
        if (roll <= source.StatusPercent)
        {
            target.ApplyBackfire(type);
        }

        ApplyCardEffectsClientRpc(targetId, type);
    }

    [ClientRpc]
    private void ApplyCardEffectsClientRpc(int targetId, CardType type)
    {
        NetworkPlayer target = Players.Find(p => p.PlayerId == targetId);
        if (target != null)
        {
            target.TriggerCardVFX(type);
        }
    }
    #endregion

    #region Bottle / Projectile
    [ServerRpc(RequireOwnership = false)]
    public void ThrowBottleServerRpc(Vector3 start, Vector3 target)
    {
        GameObject bottle = Instantiate(BottlePrefab, start, Quaternion.identity);
        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        rb.velocity = (target - start) / BaseWaitTime - 0.5f * Physics.gravity * BaseWaitTime;
        bottle.GetComponent<NetworkObject>().Spawn();
    }
    #endregion
}