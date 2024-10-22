using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodlossSystem : MonoBehaviour
{
    //!- Coded By Charlie -!

    public static BloodlossSystem Instance;

    //1 is Easy, 2 is Hard
    int difficulty;

    [Header("Countdown References")]
    [SerializeField] Image countdownImage;
    [SerializeField] float easyCountdownTime = 12f;
    [SerializeField] float hardCountdownTime = 6f;

    float countdownTime;
    float currentTime;
    bool isCountingDown = false;
    bool bloodlossEffectsEnabled;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        difficulty = DifficultyManager.Instance.difficulty;

        //Only Enable Blood Loss Side Effects If It's Hard Mode
        if(difficulty == 1)
        {
            bloodlossEffectsEnabled = false;
            countdownTime = easyCountdownTime;
        }
        else
        {
            bloodlossEffectsEnabled = true;
            countdownTime = hardCountdownTime;
        }

        Debug.Log("Difficulty: " + difficulty);
    }

    void Update()
    {
        if (isCountingDown)
        {
            //Decrease Time
            currentTime -= Time.deltaTime;
            //Normalize The Time To 0-1 Range So It Fits In Image Right
            float fillAmount = Mathf.Clamp01(currentTime / countdownTime);
            //Fill Bar Visual
            countdownImage.fillAmount = fillAmount;

            if (currentTime <= 0f)
            {
                //Ended Countdown, Die
                currentTime = 0f;
                isCountingDown = false;
                CountdownFinished();
            }
        }
    }

    public void StartCountdown()
    {
        currentTime = countdownTime;
        isCountingDown = true;
        countdownImage.fillAmount = 1f;
    }

    void CountdownFinished()
    {
        Debug.Log("You Lost All Your Blood :(");

        //Reload Scene
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }


}
