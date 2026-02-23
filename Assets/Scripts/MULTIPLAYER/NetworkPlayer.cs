using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class NetworkPlayer : NetworkBehaviour
{
    public int PlayerId;

    [Header("Player Stats")]
    public int Armour;
    public int Bullets;
    public bool HasGun;
    public bool HasKnife;
    public float StatusPercent = 0f;

    [HideInInspector] public PlayerStats Stats = new PlayerStats();

    [Header("Player Hand")]
    public MultiplayerHand PlayerHand;

    //TURN STATE
    private NetworkVariable<bool> isMyTurn = new NetworkVariable<bool>(false);
    public bool IsMyTurn => isMyTurn.Value; //read only for others

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerId = (int)OwnerClientId;
        }
    }

    //Called by TurnManager to start this player's turn
    public void StartTurn()
    {
        if (!IsOwner) return;

        isMyTurn.Value = true;
        PlayerHand.StartKnifeAction();
    }

    [ClientRpc]
    public void StartTurnClientRpc()
    {
        if (IsOwner)
        {
            isMyTurn.Value = true;
            PlayerHand.StartKnifeAction();
        }
    }

    //Called by TurnManager or other clients to end the turn
    public void EndTurn()
    {
        if (!IsOwner) return;

        isMyTurn.Value = false;
        PlayerHand.DisableCameraAfterAction();
    }

    [ClientRpc]
    public void EndTurnClientRpc()
    {
        if (IsOwner)
        {
            isMyTurn.Value = false;
            PlayerHand.DisableCameraAfterAction();
        }
    }

    //CARD/ BACKFIRE SYSTEM
    public void TriggerCardVFX(CardType type)
    {
        VisualEffect vfx = GetComponentInChildren<VisualEffect>();
        if (vfx != null) vfx.Play();
    }

    public void ApplyBackfire(CardType type)
    {
        switch (type)
        {
            case CardType.Knife: Stats.KnifeBackfire++; break;
            case CardType.Armour: Stats.ArmourBackfire++; break;
            case CardType.Cigar: Stats.CigarBackfire++; break;
            case CardType.EmptyPromise: Stats.EmptyPromiseBackfire++; break;
        }
    }
}

[System.Serializable]
public class PlayerStats
{
    public int KnifePlayed;
    public int KnifeBackfire;
    public int ArmourUsed;
    public int ArmourBackfire;
    public int CigarUsed;
    public int CigarBackfire;
    public int EmptyPromiseUsed;
    public int EmptyPromiseBackfire;
    public int BottleUsed;
    public int BottleBackfire;
}