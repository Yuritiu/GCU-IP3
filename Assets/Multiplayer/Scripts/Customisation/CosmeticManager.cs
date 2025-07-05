using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public CosmeticPreviewRotator cosmeticPreviewRotator;
    [SerializeField] public GameObject playerModel;
    private CosmeticData currentEquippedData;

    [Header("UI References")]
    [SerializeField] public Transform gridParent;
    [SerializeField] public GameObject cosmeticSlotPrefab;
    [SerializeField] public ScrollRect scrollRect;

    [Header("Cosmetics")]
    [SerializeField] public List<CosmeticData> allCosmetics;

    private GameObject equippedCosmetic;
    private GameObject equippedSlotUI;
    private string localPlayerID;
    private DeveloperIdentityManager devManager;
    private PlayerIdentityManager idManager;

    void Start()
    {
        devManager = FindObjectOfType<DeveloperIdentityManager>();
        idManager = FindObjectOfType<PlayerIdentityManager>();
        localPlayerID = idManager != null ? idManager.PlayerID : "";

        DisplayCosmetics();
    }

    void DisplayCosmetics()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        var sortedCosmetics = allCosmetics
            .Where(cosmetic => cosmetic.rarity != RarityLevel.DeveloperOnly ||
                               (devManager != null && devManager.IsDeveloper(localPlayerID)))
            .OrderByDescending(c => c.rarity)
            .ToList();

        foreach (var cosmetic in sortedCosmetics)
        {
            var slot = Instantiate(cosmeticSlotPrefab, gridParent);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = cosmetic.icon;
            slot.transform.Find("Name").GetComponent<TMP_Text>().text = cosmetic.itemName;

            var rarityText = slot.transform.Find("Rarity").GetComponent<TMP_Text>();
            rarityText.text = cosmetic.rarity.ToString();
            rarityText.color = GetColorByRarity(cosmetic.rarity);

            slot.transform.Find("Equipped").gameObject.SetActive(false);

            var button = slot.transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => EquipCosmetic(cosmetic, slot));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridParent.GetComponent<RectTransform>());
    }

    void EquipCosmetic(CosmeticData data, GameObject slotUI)
    {
        if (equippedCosmetic != null)
            Destroy(equippedCosmetic);

        //Find The Attatchment Point Within This GameObject
        Transform attachmentPoint = FindChildRecursive(playerModel.transform, data.attachPointName);

        if (attachmentPoint == null)
        {
            Debug.LogWarning($"Attachment point '{data.attachPointName}' not found on player model.");
            return;
        }

        equippedCosmetic = Instantiate(data.prefab, attachmentPoint);
        equippedCosmetic.transform.localPosition = Vector3.zero;
        equippedCosmetic.transform.localRotation = Quaternion.identity;

        if (equippedSlotUI != null)
            equippedSlotUI.transform.Find("Equipped").gameObject.SetActive(false);

        slotUI.transform.Find("Equipped").gameObject.SetActive(true);
        slotUI.transform.Find("Equipped").GetComponent<TMP_Text>().color = Color.green;
        slotUI.transform.Find("Equipped").GetComponent<TMP_Text>().text = "Equipped";

        equippedSlotUI = slotUI;
    }

    private Transform FindChildRecursive(Transform parent, string targetName)
    {
        //Search Transforms For Attatchment Name
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
                return child;

            Transform result = FindChildRecursive(child, targetName);
            if (result != null)
                return result;
        }
        return null;
    }

    Color GetColorByRarity(RarityLevel rarity)
    {
        return rarity switch
        {
            RarityLevel.Common => Color.white,
            RarityLevel.Rare => Color.blue,
            RarityLevel.Epic => new Color(0.6f, 0, 0.8f),
            RarityLevel.Legendary => new Color(1f, 0.5f, 0),
            RarityLevel.DeveloperOnly => Color.red,
            _ => Color.gray,
        };
    }
}