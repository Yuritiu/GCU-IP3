using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodlossSystem : MonoBehaviour
{
    //!- Coded By Charlie & Ben-!

    public static BloodlossSystem Instance;

    //1 is Easy, 2 is Hard
    int difficulty;

    [Header("Countdown References")]
    [SerializeField] Image countdownImage;
    [SerializeField] Image bloodBlur;
    float easyCountdownTime = 400f;
    float hardCountdownTime = 350f;

    [Header("Audio References")]
    [SerializeField] private AudioSource heartbeat;
    [SerializeField] private AudioSource heartbeatfast;// faster version when low
    [SerializeField] private AudioClip thud;
    bool heartbeatStartCalled;
    bool heartbeatfastStartCalled;
    

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
        if (DifficultyManager.Instance != null)
        {
            difficulty = DifficultyManager.Instance.difficulty;
        }
        else
        {
            difficulty = 1;
        }

        bloodlossEffectsEnabled = true;

        if (difficulty == 0)
        {
            maxHealth = easyCountdownTime;
        }
        else
        {
            maxHealth = hardCountdownTime;
        }

        currentHealth = maxHealth;
        countdownImage.fillAmount = 1f;

        //Debug.Log("Difficulty: " + difficulty);
    }
    private void Update()
    {
        //Normalize The Time To 0-1 Range So It Fits In Image Right
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        //Fill Bar Visual
        countdownImage.fillAmount = fillAmount;
        //print(currentHealth);
        if (bloodlossEffectsEnabled)
        {
            Color bloodBlurColour = bloodBlur.color;
            bloodBlurColour.a = 0.7f - (fillAmount * 1.5f);
            bloodBlur.color = bloodBlurColour;
            //print(bloodBlur.color.a);
        }

        //Play Heartbeat SFX
        if(fillAmount <= 0.75f && !heartbeatStartCalled)
        {
            heartbeatStartCalled = true;
            
            heartbeat.Play();
            
        }

        if(fillAmount <= 0.5f && !heartbeatfastStartCalled)
        {
            heartbeatfastStartCalled = true;

            heartbeat.Stop();
            
            heartbeatfast.Play();
        }
    }

    void FixedUpdate()
    {
        if (isCountingDown)
        {
            //Decrease Time
            currentHealth -= bloodlossTime;
        
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
        //increases the speed of bloodloss
        isCountingDown = true;
        bloodlossTime += knifeBloodlossAdd;
    }

    void CountdownFinished()
    {
        Debug.Log("You Lost All Your Blood :(");

        heartbeatfast.Stop();
        SFXManager.instance.PlaySFXClip(thud, transform, 1f);

        GameManager.Instance.EndGameLose();
    }
}
