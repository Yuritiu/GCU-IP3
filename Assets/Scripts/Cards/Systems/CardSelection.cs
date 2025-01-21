using TMPro;
using UnityEngine;

public class CardSelection : MonoBehaviour
{
    [Header("Info Variables")]
    [SerializeField] private TextMeshProUGUI cardInfoText;

    [HideInInspector] public bool canSelect = true;

    public GameSettingsManager gamesSettingsManager;

    private static CardSelection currentlyHoveredCard;

    void Start()
    {
        if (cardInfoText != null)
            cardInfoText.gameObject.SetActive(false);

        gamesSettingsManager = FindFirstObjectByType<GameSettingsManager>();
    }

    public void CardHovered(bool hovering)
    {
        if (CardDrawSystem.Instance.cardMoving || cardInfoText == null)
            return;

        float cardZPosition = transform.position.z;
        if (cardZPosition > 0)
        {
            hovering = false;
        }

        if (hovering)
        {
            if (currentlyHoveredCard != null && currentlyHoveredCard != this)
            {
                currentlyHoveredCard.cardInfoText.gameObject.SetActive(false);
                currentlyHoveredCard = null;
            }
            currentlyHoveredCard = this;
        }
        else
        {
            if (currentlyHoveredCard == this)
                currentlyHoveredCard = null;
        }

        if (gameObject.name == "Discarded Card")
        {
            hovering = false;
        }

        if (gamesSettingsManager.assistsOn == true)
        {
            cardInfoText.gameObject.SetActive(hovering);
        }
        else
        {
            cardInfoText.gameObject.SetActive(false);
        }
    }

    public static void ClearAllHovers()
    {
        if (currentlyHoveredCard != null)
        {
            currentlyHoveredCard.cardInfoText.gameObject.SetActive(false);
            currentlyHoveredCard = null;
        }
    }
}
