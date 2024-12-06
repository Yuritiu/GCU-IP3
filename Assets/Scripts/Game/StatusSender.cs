using System.Collections;
using UnityEngine;

public class StatusSender : MonoBehaviour
{
    private StatusDropdown statusDropdown;

    private readonly (int playerIndex, int effectIndex)[] testCases = new (int, int)[]
    {
        (0, 0),
        (1, 1),
        (0, 2),
        (1, 3),
        (0, 4),
        (1, 5),
        (0, 6)
    };

    private void Start()
    {
        statusDropdown = FindObjectOfType<StatusDropdown>();
        if (statusDropdown == null)
        {
            Debug.LogWarning("Status Dropdown Not Found");
        }
        else
        {
            StartCoroutine(SendTestCases());
        }
    }

    private IEnumerator SendTestCases()
    {
        foreach (var (playerIndex, effectIndex) in testCases)
        {
            if (statusDropdown != null)
            {
                statusDropdown.DisplayStatusEffect(playerIndex, effectIndex);
            }
            yield return new WaitForSeconds(5.0f);
        }
    }
}
