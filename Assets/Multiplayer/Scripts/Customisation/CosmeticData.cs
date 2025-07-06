using UnityEngine;

public enum CosmeticType
{
    Hat,
    Glasses
}

public enum RarityLevel
{
    Common,
    Rare,
    Epic,
    Legendary,
    Developer
}

[CreateAssetMenu(menuName = "Cosmetics/Cosmetic Item")]
public class CosmeticData : ScriptableObject
{
    public string itemName;
    public RarityLevel rarity;
    public Sprite icon;
    public GameObject prefab;
    public string attachPointName;
    public CosmeticType cosmeticType;
}