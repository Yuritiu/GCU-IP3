using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.VFX;

public class ShootScript : MonoBehaviour
{

    private StatusDropdown statusDropdown;

    public static ShootScript instance1;
    public static ShootScript instance2;
    public float delay;
    public bool firePressed;
    public Animator gunAnim;
    public ParticleSystem Flash;

    //backfire effects
    public ParticleSystem gunBackfire1;
    public ParticleSystem gunBackfire2;
    public ParticleSystem gunBackfire3;
    public ParticleSystem gunBackfire4;
    public ParticleSystem gunBackfire5;
    public ParticleSystem gunBackfire6;



    public int PRandom;
    public int AiRandom;
    public bool PlayerShot;
    public bool AiShot;
    public string gunName;
    public GameObject Hammer;
    float startingRotation = 38f;
    float currentRotation;
    public bool clampActivated;
    public TextMeshProUGUI textUnderGun;

    bool isPlayer;
    bool reducedPlayerGunCount = false;
    bool hasGunLoaded= false;

    private GameManager gameManager;
    [SerializeField] private AudioClip Gunload;
    [SerializeField] private AudioClip Gunfire;
    [SerializeField] private AudioClip earRinging;
    [SerializeField] private AudioClip emptySFX;

    public VisualEffect emptyGunVFX;
    private BackfireFlash backfireFlash;



    //public CameraController cameraController;

    private void Start()
    {
        gunAnim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();
        currentRotation = startingRotation;
        backfireFlash = FindObjectOfType<BackfireFlash>();
    }

    private void Awake()
    {
        if (gunName == "PlayerGun")
        {
            //print("instanceed1");
            instance1 = this;

            isPlayer = false;
        }
        if(gunName == "AiGun")
        {
            instance2 = this;
            isPlayer = true;
        }

        
    }

    private void OnEnable()
    {
        //cameraController.gunInHand = true;

        if (gunName == "AiGun")
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(AiFire(gameObject));
            //cameraController.gunInHand = false;
        }
    }
   

    private void Update()
    {
        //Debug.Log("Hey We Got Here!");
        if (gunName == "PlayerGun" && Input.GetMouseButtonDown(0) && firePressed == false && currentRotation <= 0) //&& !reducedPlayerGunCount)
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(Fire(gameObject));
            Hammer.transform.Rotate(38f, 0, 0);
            currentRotation = 38;

            //reducedPlayerGunCount = true;
            gameManager.inGunAction = false;
            hasGunLoaded = false;

            //gameManager.playerGunCount = 0;
        }

        if (gunName == "PlayerGun" && Input.GetMouseButton(1) && currentRotation > 0)
        {
            Hammer.transform.Rotate(-1, 0f, 0f, Space.Self);
            currentRotation = currentRotation - 1;
            if (!hasGunLoaded) 
                                 
            {
                SFXManager.instance.PlaySFXClip(Gunload, transform, 0.15f);
                hasGunLoaded = true; 
            }
        }


        if (currentRotation > 0 && gunName == "PlayerGun" && firePressed ==false) ;
        {
            textUnderGun.text = "HOLD RMB";
        }

        if(currentRotation <= 0 && gunName == "PlayerGun")
        {
            textUnderGun.text = "CLICK LMB";
        }
    }

    private IEnumerator AiFire(GameObject gun)
    {
        OpponentAnimationController animController = FindAnyObjectByType<OpponentAnimationController>();

        AiShot = true;
        
        int randForBullet = UnityEngine.Random.Range(1, 7);

        AiRandom = randForBullet;

        //AiRandom = 8;

        //UnityEngine.Debug.Log("bullet " + GameManager.Instance.bullets);
        //UnityEngine.Debug.Log("random " + AiRandom);
        animController.GunTr();
        yield return new WaitForSeconds(1.5f);
        firePressed = true;
        gunAnim.Play("Airecoil");
        if (AiRandom <= GameManager.Instance.bullets)
        {
            Flash.Play();
            SFXManager.instance.PlaySFXClip(Gunfire, transform, 0.3f);
        }
        else
        {
            if (emptyGunVFX != null)
            {

                emptyGunVFX.Play();
                SFXManager.instance.PlaySFXClip(emptySFX, transform, 0.3f);
            }
        }
        yield return new WaitForSeconds(delay);

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance && (GameManager.Instance.bullets > 0))

        {
            //Shoots off your own finger
            GameManager.Instance.ReduceHealth(1, 3);
            GunBackfire();
            
            statusDropdown.DisplayStatusEffect(1, 1);

            
        }
        else if (AiRandom <= GameManager.Instance.bullets)
        {
            GameManager.Instance.CheckArmour(2, 3);
            GameManager.Instance.bullets--;
            
        }

        GameManager.Instance.Gun.SetActive(true);
        GameManager.Instance.aiGunActive = false;
        GameManager.Instance.isAiGun = false;


       
        gameManager.inGunAction = false;

        gameManager.aiGunCount = 0;

        //if other card is gun then wait
        if (GameManager.Instance.has2Guns == false)
        {
            GameManager.Instance.inGunAction = false;
        }
        //Switches opponet back to idle
        //animController.IdleTr();
        gameObject.SetActive(false);
        gunAnim.Play("GunPause");
        firePressed = false;
        AiShot = false;
        
    }

    private IEnumerator Fire(GameObject gun)
    {
        //print("before null check");
        
        
        PlayerShot = true;

        //print("not null");
        int rand = UnityEngine.Random.Range(0, 5);
            
        int randForBullet = UnityEngine.Random.Range(1, 7);

        PRandom = randForBullet;
        
        //rand = 1;
        //PRandom = 1;
        
        firePressed = true;
        gunAnim.Play("recoil");
        if (PRandom <= GameManager.Instance.bullets)
        {
            
            Flash.Play();
            SFXManager.instance.PlaySFXClip(Gunfire, transform, 0.3f);
        }
        else
        {
            if (emptyGunVFX != null)
            {
                
                emptyGunVFX.Play(); 
                SFXManager.instance.PlaySFXClip(emptySFX, transform, 0.3f);
            }
        }
       
        yield return new WaitForSeconds(delay);

        if (rand == 0 && GameManager.Instance.bullets > 0)

        {
                //Shoots off your own finger
            GameManager.Instance.ReduceHealth(2, 3);
            statusDropdown.DisplayStatusEffect(0, 1);
            GunBackfire();
            backfireFlash.BackfireActive();
            SFXManager.instance.PlaySFXClip(earRinging, transform, 0.3f);

        }
        else if (PRandom <= GameManager.Instance.bullets)
        {
            GameManager.Instance.CheckArmour(1, 3);
            GameManager.Instance.bullets--;
        }

        GameManager.Instance.playerGunActive = false;
        GameManager.Instance.Gun.SetActive(true);
        GameManager.Instance.playerGunActive = false;
        GameManager.Instance.isPlayerGun = false;
        if (GameManager.Instance.has2Guns == false)
        {
            GameManager.Instance.inGunAction = false;
            PlayerShot = false;
        }
        gunAnim.Play("GunPause");
        gameObject.SetActive(false);
        firePressed = false;

        //cameraController.gunInHand = false;
    }

    private void GunBackfire()
    {
        gunBackfire1.Play();
        gunBackfire2.Play();
        gunBackfire3.Play();
        gunBackfire4.Play();
        gunBackfire5.Play();
        gunBackfire6.Play();
    }
    
   
}
