using UnityEngine;

public enum CardType { Knife, Gun, Armour, Cigar, EmptyPromise, Bottle }

public interface ICardAction
{
    CardType CardType { get; }
    void PlayCardForPlayer(NetworkPlayer player);
    void PlayCardForOpponent(NetworkPlayer target);
}