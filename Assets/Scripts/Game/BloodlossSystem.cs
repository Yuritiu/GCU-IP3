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
    [SerializeField] Image bloodBlur;
    float easyCountdownTime = 400f;
    float hardCountdownTime = 200f;

    float maxHealth;
    float currentHealth;
    bool isCountingDown = false;
    bool bloodlossEffectsEnabled;
    [HideInInspector] public float bloodlossTime = 0f;
    [SerializeField] private float knifeBloodlossAdd = 0.05f;
    public float shieldBloodlossReduce = 0.025f;

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
            maxHealth = easyCountdownTime;
        }
        else
        {
            bloodlossEffectsEnabled = true;
            maxHealth = hardCountdownTime;
        }
        currentHealth = maxHealth;
        countdownImage.fillAmount = 1f;

        //Debug.Log("Difficulty: " + difficulty);
    }

    void FixedUpdate()
    {
        if (isCountingDown)
        {
            //Decrease Time
            currentHealth -= bloodlossTime;
            //Normalize The Time To 0-1 Range So It Fits In Image Right
            float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
            //Fill Bar Visual
            countdownImage.fillAmount = fillAmount;
            print(currentHealth);
            if (bloodlossEffectsEnabled)
            {
                Color bloodBlurColour = bloodBlur.color;
                bloodBlurColour.a = 0.7f - (fillAmount*1.5f);
                bloodBlur.color = bloodBlurColour;
                //print(bloodBlur.color.a);
            }

            if (currentHealth <= 0f)
            {
                //Ended Countdown, Die
                currentHealth = 0f;
                isCountingDown = false;
                CountdownFinished();
            }
        }
    }

    public void IncreaseBloodloss()
    {
        isCountingDown = true;
        bloodlossTime += knifeBloodlossAdd;
    }

    void CountdownFinished()
    {
        Debug.Log("You Lost All Your Blood :(");

        //Reload Scene
        var activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(activeScene);
    }


}
