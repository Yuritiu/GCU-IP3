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
    [SerializeField] Image bloodBlur;
    [SerializeField] Image blackoutBlur;
    float easyCountdownTime = 400f;
    float hardCountdownTime = 350f;

    [Header("Audio References")]
    [SerializeField] private AudioSource heartbeat;
    [SerializeField] private AudioSource heartbeatfast;// faster version when low
    [SerializeField] private AudioClip thud;
    bool heartbeatStartCalled;
    bool heartbeatfastStartCalled;
    bool usingHeartBeat1;
    bool usingHeartBeat2;
    

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

        //Debug.Log("Difficulty: " + difficulty);
    }
    private void Update()
    {
        //Normalize The Time To 0-1 Range So It Fits In Image Right
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        //Fill Bar Visual
        //print(currentHealth);
        print(fillAmount);
        //print(heartbeat.volume);
        //print(heartbeatfast.volume);
        if (bloodlossEffectsEnabled)
        {
            Color bloodBlurColour = bloodBlur.color;
            bloodBlurColour.a = 0.6f - (fillAmount);
            bloodBlur.color = bloodBlurColour;
            //print(bloodBlur.color.a);

            Color blackoutBlurColour = blackoutBlur.color;
            blackoutBlurColour.a = 0.2f - (fillAmount * 5f);
            blackoutBlur.color = blackoutBlurColour;
            //print(bloodBlur.color.a);

        }



        if (usingHeartBeat1)
        {
            heartbeat.volume = 0.75f  - fillAmount;
        }
        
        if(usingHeartBeat2)
        {
            heartbeatfast.volume = 0.5f  - fillAmount;
        }


        //Play Heartbeat SFX
        if (fillAmount <= 0.75f && !heartbeatStartCalled)
        {
            usingHeartBeat1 = true;
            heartbeatStartCalled = true;
            heartbeat.Play();
        }

        if(fillAmount <= 0.35f && !heartbeatfastStartCalled)
        {
            usingHeartBeat1 = false;
            usingHeartBeat2 = true;
            
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
        bloodlossTime += knifeBloodlossAdd * 5;
    }

    void CountdownFinished()
    {
        Debug.Log("You Lost All Your Blood :(");

        heartbeatfast.Stop();
        SFXManager.instance.PlaySFXClip(thud, transform, 1f);

        GameManager.Instance.EndGameLose();
    }
}
