using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance { get; private set; }

    Dictionary<string, int> stats = new Dictionary<string, int>();

    TextMeshProUGUI statsDisplay;
    Button statsButton;
    Button resetStatsButton;
    Image bg;

    bool menuOpened = false;
    bool inMainMenu = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        menuOpened = false;

        InitializeStats();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            inMainMenu = true;
            statsDisplay = GameObject.Find("StatsTextFind")?.GetComponent<TextMeshProUGUI>();
            bg = GameObject.Find("BackgroundFind")?.GetComponent<Image>();

            statsButton = GameObject.Find("StatsButtonFind")?.GetComponent<Button>();
            resetStatsButton = GameObject.Find("ResetStatsButtonFind")?.GetComponent<Button>();

            if (statsButton != null)
            {
                statsButton.onClick.AddListener(OpenStatsMenu);
            }

            if (resetStatsButton != null)
            {
                resetStatsButton.onClick.AddListener(ResetStats);
            }
        }
        else
        {
            inMainMenu = false;
        }
    }

    void Update()
    {
        if (inMainMenu && menuOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuOpened = false;
            }
        }

        if (inMainMenu && !menuOpened)
        {
            bg.enabled = false;
            statsDisplay.enabled = false;
            resetStatsButton.interactable = false;
            resetStatsButton.gameObject.GetComponent<Image>().enabled = false;
            resetStatsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
    }

    public void OpenStatsMenu()
    {
        UpdateStatsUI();
    }

    void InitializeStats()
    {
        stats["Wins"] = 0;
        stats["Deaths"] = 0;
        stats["TotalCardsPlayed"] = 0;
        //stats["FavouriteCard"] = 0;
        stats["KnifeCardsPlayed"] = 0;
        stats["GunCardsPlayed"] = 0;
        stats["ArmourCardsPlayed"] = 0;
        stats["BottleCardsPlayed"] = 0;
        stats["CigarCardsPlayed"] = 0;
        stats["OneInTheChamberCardsPlayed"] = 0;
        stats["EmptyPromiseCardsPlayed"] = 0;

        LoadStats();
    }

    public void UpdateStat(string statName, int amount)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] += amount;
        }
        else
        {
            stats[statName] = amount;
        }

        //If A Card Stat Is Updated -> Calculate TotalCardsPlayed
        if (IsCardStat(statName))
        {
            UpdateTotalCardsPlayed();
        }

        SaveStats();
        PlayerPrefs.Save();
    }

    bool IsCardStat(string statName)
    {
        return statName == "KnifeCardsPlayed" ||
               statName == "GunCardsPlayed" ||
               statName == "ArmourCardsPlayed" ||
               statName == "BottleCardsPlayed" ||
               statName == "CigarCardsPlayed" ||
               statName == "OneInTheChamberCardsPlayed" ||
               statName == "EmptyPromiseCardsPlayed";
    }

    void UpdateTotalCardsPlayed()
    {
        stats["TotalCardsPlayed"] = stats["KnifeCardsPlayed"] +
                                    stats["GunCardsPlayed"] +
                                    stats["ArmourCardsPlayed"] +
                                    stats["BottleCardsPlayed"] +
                                    stats["CigarCardsPlayed"] +
                                    stats["OneInTheChamberCardsPlayed"] +
                                    stats["EmptyPromiseCardsPlayed"];
    }

    public int GetStat(string statName)
    {
        return stats.ContainsKey(statName) ? stats[statName] : 0;
    }

    void SaveStats()
    {
        foreach (var stat in stats)
        {
            PlayerPrefs.SetInt(stat.Key, stat.Value);
        }
        PlayerPrefs.Save();
    }

    void LoadStats()
    {
        stats["Wins"] = PlayerPrefs.GetInt("Wins", 0);
        stats["Deaths"] = PlayerPrefs.GetInt("Deaths", 0);
        stats["TotalCardsPlayed"] = PlayerPrefs.GetInt("TotalCardsPlayed", 0);
        //stats["FavouriteCard"] = PlayerPrefs.GetString("FavouriteCard");
        stats["KnifeCardsPlayed"] = PlayerPrefs.GetInt("KnifeCardsPlayed", 0);
        stats["GunCardsPlayed"] = PlayerPrefs.GetInt("GunCardsPlayed", 0);
        stats["ArmourCardsPlayed"] = PlayerPrefs.GetInt("ArmourCardsPlayed", 0);
        stats["BottleCardsPlayed"] = PlayerPrefs.GetInt("BottleCardsPlayed", 0);
        stats["CigarCardsPlayed"] = PlayerPrefs.GetInt("CigarCardsPlayed", 0);
        stats["OneInTheChamberCardsPlayed"] = PlayerPrefs.GetInt("OneInTheChamberCardsPlayed", 0);
        stats["EmptyPromiseCardsPlayed"] = PlayerPrefs.GetInt("EmptyPromiseCardsPlayed", 0);
    }

    public void UpdateStatsUI()
    {
        statsDisplay.enabled = true;
        bg.enabled = true;
        menuOpened = true;
        resetStatsButton.interactable = true;
        resetStatsButton.gameObject.GetComponent<Image>().enabled = true;
        resetStatsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

        if (statsDisplay != null)
        {
            statsDisplay.text = 
                $"Wins: {GetStat("Wins")}\n" +
                $"Deaths: {GetStat("Deaths")}\n\n" +
                $"Total Cards Played: {GetStat("TotalCardsPlayed")}\n" +
                //$"Favourite Card: {GetStat("FavouriteCard")}\n" +
                $"Knife Cards Played: {GetStat("Knife Cards Played")}\n" +
                $"Gun Cards Played: {GetStat("GunCardsPlayed")}" +
                $"\nArmour Cards Played: {GetStat("ArmourCardsPlayed")}\n" +
                $"Bottle Cards Played: {GetStat("BottleCardsPlayed")}\n" +
                $"Cigar Cards Played: {GetStat("CigarCardsPlayed")}" +
                $"\nOne In The Chamber Cards Played: {GetStat("OneInTheChamberCardsPlayed")}\n" +
                $"Empty Promise Cards Played: {GetStat("EmptyPromiseCardsPlayed")}";
        }
    }

    public void ResetStats()
    {
        stats["Wins"] = 0;
        stats["Deaths"] = 0;
        stats["TotalCardsPlayed"] = 0;
        stats["KnifeCardsPlayed"] = 0;
        stats["GunCardsPlayed"] = 0;
        stats["ArmourCardsPlayed"] = 0;
        stats["BottleCardsPlayed"] = 0;
        stats["CigarCardsPlayed"] = 0;
        stats["OneInTheChamberCardsPlayed"] = 0;
        stats["EmptyPromiseCardsPlayed"] = 0;

        SaveStats();
        UpdateStatsUI();
    }
}
