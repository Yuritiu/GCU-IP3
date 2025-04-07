using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodlossSystemAI : MonoBehaviour
{
  
    public static BloodlossSystemAI Instance;

    int difficulty;

   

    

   
    [SerializeField] private AudioClip thud;
    

    public float maxAIHealth = 350;
    [HideInInspector] public float currentAIHealth;
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

        

        currentAIHealth = maxAIHealth;

        //Debug.Log("Difficulty: " + difficulty);
    }
    private void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (isCountingDown)
        {
            //Decrease Time
            currentAIHealth -= bloodlossTime;

            if (currentAIHealth <= 0f)
            {
                //Ended Countdown, Die
                currentAIHealth = 0f;
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

       
        SFXManager.instance.PlaySFXClip(thud, transform, 1f);
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.bloodLost = true;
        GameManager.Instance.EndGameWin();
    }
}
