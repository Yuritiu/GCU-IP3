using UnityEngine;

public class KnifeCard : MonoBehaviour, ICardAction
{
    public CardType CardType => CardType.Knife;
    [SerializeField] private AudioClip backfireSFX;
    private BackfireGlow glow;

    private void Awake()
    {
        glow = GetComponent<BackfireGlow>();
    }

    public void PlayCardForPlayer(NetworkPlayer player)
    {
        player.Stats.KnifePlayed++;
        float roll = Random.Range(0f, 100f);
        if (roll <= player.StatusPercent)
        {
            player.ApplyBackfire(CardType);
            if (glow != null) glow.GlowActive();
            AudioSource.PlayClipAtPoint(backfireSFX, player.transform.position, 0.2f);
        }
    }

    public void PlayCardForOpponent(NetworkPlayer target)
    {
        target.Stats.KnifePlayed++;
    }
}