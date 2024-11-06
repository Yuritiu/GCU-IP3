using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSender : MonoBehaviour
{
    private StatusDropdown statusDropdown;

    void Start()
    {
        statusDropdown = FindObjectOfType<StatusDropdown>();
        if (statusDropdown == null)
        {
            Debug.LogWarning("Status Dropdown Not Found");
        }
    }

    void Update()
    {
        if (statusDropdown == null) return;

        for (int i = 1; i <= 7; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                statusDropdown.DisplayStatusEffect(i - 1);
            }
        }
    }
}
