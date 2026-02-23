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
    [SerializeField] private CosmeticType currentCategory = CosmeticType.Hat;
    private Dictionary<CosmeticType, GameObject> equippedCosmetics = new();
    private Dictionary<CosmeticType, GameObject> equippedSlotUIs = new();
    private Dictionary<CosmeticType, CosmeticData> equippedDataByType = new();

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

    void EquipCosmetic(CosmeticData data, GameObject slotUI)
    {
        var type = data.cosmeticType;

        //Check If The Clicked Cosmetic Is Already Equipped
        if (equippedDataByType.TryGetValue(type, out var currentlyEquippedData) && currentlyEquippedData == data)
        {
            //Unequip Logic
            if (equippedCosmetics.TryGetValue(type, out var toRemove) && toRemove != null)
                Destroy(toRemove);

            if (equippedSlotUIs.TryGetValue(type, out var uiSlot) && uiSlot != null)
                uiSlot.transform.Find("Equipped").gameObject.SetActive(false);

            equippedCosmetics.Remove(type);
            equippedSlotUIs.Remove(type);
            equippedDataByType.Remove(type);

            return;
        }

        //Only Destroy Cosmetic If Same Type e.g. can't have 2 hats but can have a hat and glasses
        if (equippedCosmetics.TryGetValue(type, out var oldCosmetic))
            Destroy(oldCosmetic);

        //Find The Attatchment Point Within This GameObject
        Transform attachmentPoint = FindChildRecursive(playerModel.transform, data.attachPointName);

        if (attachmentPoint == null)
        {
            Debug.LogError($"Attachment point '{data.attachPointName}' not found on player model.");
            return;
        }

        equippedCosmetic = Instantiate(data.prefab, attachmentPoint);
        equippedCosmetic.transform.localPosition = Vector3.zero;
        equippedCosmetic.transform.localRotation = Quaternion.identity;

        //Keep Hat "Equipped" Text Active If Also Equip Glasses
        if (equippedSlotUIs.TryGetValue(data.cosmeticType, out var previousSlotUI) && previousSlotUI != null && previousSlotUI != slotUI)
        {
            previousSlotUI.transform.Find("Equipped").gameObject.SetActive(false);
        }

        slotUI.transform.Find("Equipped").gameObject.SetActive(true);
        slotUI.transform.Find("Equipped").GetComponent<TMP_Text>().color = Color.green;
        slotUI.transform.Find("Equipped").GetComponent<TMP_Text>().text = "Equipped";

        equippedCosmetics[type] = equippedCosmetic;
        equippedSlotUIs[type] = slotUI;
        equippedDataByType[type] = data;
        currentEquippedData = data;

        equippedSlotUI = slotUI;
    }

    void DisplayCosmetics()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        var sortedCosmetics = allCosmetics
        .Where(cosmetic => cosmetic.cosmeticType == currentCategory &&
        (cosmetic.rarity != RarityLevel.Developer ||
         (devManager != null && devManager.IsDeveloper(localPlayerID)))).OrderByDescending(c => c.rarity).ToList();

        foreach (var cosmetic in sortedCosmetics)
        {
            var slot = Instantiate(cosmeticSlotPrefab, gridParent);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = cosmetic.icon;
            slot.transform.Find("Name").GetComponent<TMP_Text>().text = cosmetic.itemName;

            var rarityText = slot.transform.Find("Rarity").GetComponent<TMP_Text>();
            rarityText.text = cosmetic.rarity.ToString();
            rarityText.color = GetColorByRarity(cosmetic.rarity);

            var equippedIndicator = slot.transform.Find("Equipped").gameObject;
            equippedIndicator.SetActive(false);

            //Reshow Equipped Text If This Cosmetic Is Equipped For This Type
            if (equippedDataByType.TryGetValue(currentCategory, out var equippedData) && equippedData == cosmetic)
            {
                equippedIndicator.SetActive(true);
                equippedIndicator.GetComponent<TMP_Text>().color = Color.green;
                equippedIndicator.GetComponent<TMP_Text>().text = "Equipped";

                equippedSlotUIs[currentCategory] = slot;
            }

            var button = slot.transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(() => EquipCosmetic(cosmetic, slot));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridParent.GetComponent<RectTransform>());
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
            RarityLevel.Developer => Color.red,
            _ => Color.gray,
        };
    }

    #region UI Buttons
    public void SetCosmeticCategory(CosmeticType newCategory)
    {
        currentCategory = newCategory;
        DisplayCosmetics();
    }

    public void OnHatTabClicked() => SetCosmeticCategory(CosmeticType.Hat);
    public void OnGlassesTabClicked() => SetCosmeticCategory(CosmeticType.Glasses);
    #endregion
}