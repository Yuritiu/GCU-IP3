using UnityEngine;

public enum RarityLevel
{
    Common,
    Rare,
    Epic,
    Legendary,
    DeveloperOnly
}

[CreateAssetMenu(menuName = "Cosmetics/Cosmetic Item")]
public class CosmeticData : ScriptableObject
{
    public string itemName;
    public RarityLevel rarity;
    public Sprite icon;
    public GameObject prefab;
    public string attachPointName;
}