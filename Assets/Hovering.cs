using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverText : MonoBehaviour
{
    public TextMeshProUGUI hoverText; // Assign in Inspector

    void Start()
    {
        hoverText.gameObject.SetActive(false); // Hide text at start
    }

    void OnMouseEnter()
    {
        if (GameManager.Instance.canPlay)
        {
            hoverText.gameObject.SetActive(true); // Show text when hovering
        }
        
    }

    void OnMouseExit()
    {
        hoverText.gameObject.SetActive(false); // Hide text when mouse leaves
    }
}