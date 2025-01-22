using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [SerializeField] GameObject statsMenu;
    [SerializeField] GameObject mainMenu;

    [HideInInspector] public bool statsMenuOpen = false;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu()
    {
        statsMenuOpen = true;
        statsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        statsMenu.SetActive(false);
        mainMenu.SetActive(true);

        statsMenuOpen = false;
    }

    void Update()
    {
        if (statsMenuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
