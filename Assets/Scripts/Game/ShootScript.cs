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

    private CameraController cameraController;
    private FovLerp fovLerp;
    private GunCameraShake gunCameraShake;
    private GunShake gunShake;

    public float shakeIntensity = 0.05f;
    private Vector3 originalPosition;
    private float shakeTime = 0.0f;
    private bool isShaking = false;




    private void Start()
    {
        gunAnim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        statusDropdown = FindAnyObjectByType<StatusDropdown>();
        currentRotation = startingRotation;
        backfireFlash = FindObjectOfType<BackfireFlash>();

        cameraController = FindFirstObjectByType<CameraController>();
        fovLerp = FindFirstObjectByType<FovLerp>();
        gunCameraShake = FindFirstObjectByType<GunCameraShake>();
        gunShake = FindFirstObjectByType<GunShake>();

        originalPosition = transform.localPosition;
    }


    private void Awake()
    {
        if (gunName == "PlayerGun")
        {
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
        if (gunName == "AiGun")
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(AiFire(gameObject));
        }
    }


    private void Update()
    {
        if (gunName == "PlayerGun")
        {
            cameraController.gunInHand = true;

            if (Input.GetMouseButton(1)) //Holding right-click
            {
                fovLerp.StartLerp(); //Start lerping FOV towards the target

                if (currentRotation > 0)
                {
                    Hammer.transform.Rotate(-1, 0f, 0f, Space.Self);
                    gunCameraShake.StartShake();
                    gunShake.StartShake();
                    currentRotation = currentRotation - 1;

                    if (!hasGunLoaded)
                    {
                        SFXManager.instance.PlaySFXClip(Gunload, transform, 0.15f);
                        hasGunLoaded = true;
                    }
                }
            }
            else //Right-click released
            {
                fovLerp.StopLerp();
                gunCameraShake.StopShake();
                gunShake.StopShake();
                if (currentRotation < startingRotation)
                {
                    Hammer.transform.Rotate(1, 0f, 0f, Space.Self);
                    currentRotation = Mathf.Min(currentRotation + 1, startingRotation);
                    hasGunLoaded = false;
                }
            }

            if (Input.GetMouseButton(0) && firePressed == false && currentRotation <= 0)
            {
                gunCameraShake.StopShake();
                gunShake.StopShake();
                gunAnim = GetComponent<Animator>();
                StartCoroutine(Fire(gameObject));
                Hammer.transform.Rotate(startingRotation, 0, 0);
                currentRotation = startingRotation;

                gameManager.inGunAction = false;
                hasGunLoaded = false;
            }

            //Update text hints
            if (currentRotation > 0 && firePressed == false)
            {
                textUnderGun.text = "HOLD RMB";
            }
            else if (currentRotation <= 0)
            {
                textUnderGun.text = "CLICK LMB";
            }
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
        cameraController.gunInHand = false;

        fovLerp.StopLerp();
        gunCameraShake.StopShake();
        gunShake.StopShake();
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
