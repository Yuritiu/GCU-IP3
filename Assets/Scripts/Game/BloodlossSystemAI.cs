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
    bool isAICountingDown = false;
    bool AIbloodlossEffectsEnabled;
    [HideInInspector] public float AIbloodlossTime = 0f;
    [SerializeField] private float knifeAIBloodlossAdd = 0.0225f;
    public float shieldAIBloodlossReduce = 0.01125f;

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

        AIbloodlossEffectsEnabled = true;

        

        currentAIHealth = maxAIHealth;

        //Debug.Log("Difficulty: " + difficulty);
    }
    private void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (isAICountingDown)
        {
            //Decrease Time
            currentAIHealth -= AIbloodlossTime;

            if (currentAIHealth <= 0f)
            {
                //Ended Countdown, Die
                currentAIHealth = 0f;
                isAICountingDown = false;
                CountdownFinished();
            }
        }
    }

    public void IncreaseBloodloss()
    {
        //increases the speed of bloodloss
        isAICountingDown = true;
        AIbloodlossTime += knifeAIBloodlossAdd;
    }

    void CountdownFinished()
    {
        Debug.Log("Ai Has Died");

       
        SFXManager.instance.PlaySFXClip(thud, transform, 1f);
        DataGathering dG = FindObjectOfType<DataGathering>();
        dG.bloodLost = true;
        GameManager.Instance.EndGameWin();
    }
}
