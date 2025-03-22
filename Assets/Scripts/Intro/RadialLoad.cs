using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RadialLoad : MonoBehaviour
{
    public Image radialImage;
    public TextMeshProUGUI textLabel;
    public float fillSpeed = 0.5f;
    public float actionThreshold = 1f;

    private float currentFill = 0f;
    private Color initialColor;
    private bool actionPerformed = false;

    public LoadingManager loadingManager;

    void Start()
    {
        initialColor = radialImage.color;
        radialImage.fillAmount = 0f;
        radialImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        if (textLabel != null)
        {
            textLabel.color = new Color(textLabel.color.r, textLabel.color.g, textLabel.color.b, 0f);
        }
    }

    void Update()
    {
        bool isHoldingSpace = Input.GetKey(KeyCode.Space);
        bool isClickingMouse = Input.GetMouseButton(0); // Only affects visibility of text

        if (isHoldingSpace)
        {
            if (currentFill < 1f && !actionPerformed)
            {
                currentFill += fillSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (currentFill > 0f && !actionPerformed)
            {
                currentFill -= fillSpeed * Time.deltaTime;
            }
        }

        if (actionPerformed)
        {
            currentFill = 1f;
        }

        radialImage.fillAmount = currentFill;
        radialImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentFill);

        if (textLabel != null)
        {
            float alpha = isClickingMouse ? 1f : currentFill; // Shows skip text when clicked on
            textLabel.color = new Color(textLabel.color.r, textLabel.color.g, textLabel.color.b, alpha);
        }

        if (currentFill >= actionThreshold && !actionPerformed)
        {
            PerformAction();
        }
    }

    private void PerformAction()
    {
        loadingManager.LoadScene("Game Scene");
        actionPerformed = true;
    }

}
