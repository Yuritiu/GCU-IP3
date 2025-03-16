using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodlossSystem : MonoBehaviour
{
    //!- Coded By Charlie & Ben-!

    public static BloodlossSystem Instance;

    int difficulty;

    [Header("Countdown References")]
    [SerializeField] Image bloodBlur;
    [SerializeField] Image blackoutBlur;
    public Image[] bloodSplatter;
    
    public GameObject blur;
    public Material blurMat;
    bool blurStarted = false;

    [Header("Audio References")]
    [SerializeField] public AudioSource heartbeat;
    [SerializeField] public AudioSource heartbeatfast;// faster version when low
    [SerializeField] private AudioClip thud;
    bool heartbeatStartCalled;
    bool heartbeatfastStartCalled;
    bool usingHeartBeat1;
    bool usingHeartBeat2;

    public float maxHealth = 350;
    [HideInInspector] public float currentHealth;
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
        
        blurStarted = false;

        currentHealth = maxHealth;

        //Debug.Log("Difficulty: " + difficulty);
    }
    private void Update()
    {
        //Normalize The Time To 0-1 Range So It Fits In Image Right
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);

        //Fill Bar Visual
        //print(currentHealth);
        //print(fillAmount);
        //print(blackoutBlur.color);
        //print(heartbeat.volume);
        //print(heartbeatfast.volume);

        for (int i = 0; i < 4; i++)
        {
            if (bloodSplatter[i].color.a > 0)
            {
                Color splatter = bloodSplatter[i].color;
                splatter.a -= 0.005f * Time.deltaTime;
                bloodSplatter[i].color = splatter;
                //print(10 * Time.deltaTime);
            }
        }
        
        if (bloodlossEffectsEnabled)
        {
            Color bloodBlurColour = bloodBlur.color;
            bloodBlurColour.a = 0.6f - (fillAmount);
            bloodBlur.color = bloodBlurColour;
            //print(bloodBlur.color.a);

            //make screen darker and add blur to start taking effect
            if (fillAmount < 0.2f)
            {
                if(!blurStarted)
                {
                    blur.SetActive(true);
                    blurStarted = true;
                }

                float blurOp = (1f - (fillAmount * 5)) / 100;

                blurMat.SetFloat("_Value", Mathf.Clamp01(blurOp));

                Color blackoutBlurColour = blackoutBlur.color;
                blackoutBlurColour.a = 1f - (fillAmount * 5f);
                blackoutBlur.color = blackoutBlurColour;
                //print(bloodBlur.color.a);
            }
            else 
            {
                if(blurStarted)
                {
                    blur.SetActive(false);
                    blurStarted = false;
                }
            }
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
        bloodlossTime += knifeBloodlossAdd;
    }

    void CountdownFinished()
    {
        Debug.Log("You Lost All Your Blood :(");

        heartbeatfast.Stop();
        SFXManager.instance.PlaySFXClip(thud, transform, 1f);
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.bloodLost = true;
        GameManager.Instance.EndGameLose();
    }
}
