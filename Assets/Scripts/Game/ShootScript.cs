using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ShootScript : MonoBehaviour
{
    public static ShootScript instance1;
    public static ShootScript instance2;
    public float delay;
    public bool firePressed;
    public Animator gunAnim;
    public ParticleSystem Flash;
    public int PRandom;
    public int AiRandom;
    public bool PlayerShot;
    public bool AiShot;
    public string gunName;
    public GameObject Hammer;
    float startingRotation = 38f;
    float currentRotation;
    bool clampActivated;
    public TextMeshProUGUI textUnderGun;

    bool isPlayer;
    bool reducedPlayerGunCount = false;

    private GameManager gameManager;
    [SerializeField] private AudioClip Gunload;
    [SerializeField] private AudioClip Gunfire;

    private void Start()
    {
        gunAnim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        currentRotation = startingRotation;  
    }

    private void Awake()
    {
        if (gunName == "Ai Gun")
        {
            //print("instanceed1");
            instance1 = this;
            //SFXManager.instance.PlaySFXClip(Gunload, transform, 0.15f);
            isPlayer = false;
        }
        if(gameObject.name == "Player Gun")
        {
            instance2 = this;
            isPlayer = true;
            //SFXManager.instance.PlaySFXClip(Gunload, transform, 0.15f);
            clampActivated = true;
            
        }
    }

    private void OnEnable()
    {
        if (gameObject.name == "Ai Gun")
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(AiFire(gameObject));
            
        }
    }

    private void Update()
    {
        //Debug.Log("Hey We Got Here!");
        if (gunName == "Player Gun" && Input.GetMouseButtonDown(0) && firePressed == false && currentRotation <= 0 && !reducedPlayerGunCount)
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(Fire(gameObject));
            Hammer.transform.Rotate(38f, 0, 0);
            currentRotation = 38;

            reducedPlayerGunCount = true;
            gameManager.inGunAction = false;

            gameManager.playerGunCount = 0;
        }

        if (gunName == "Player Gun" && Input.GetMouseButton(1) && currentRotation > 0)
        {
            Hammer.transform.Rotate(-1, 0f, 0f, Space.Self);
            currentRotation = currentRotation - 1;
        }

        if(clampActivated == true)
        {
            Freelook.Instance.minX = -2;
            Freelook.Instance.maxX = 18;
            Freelook.Instance.minY = -5;
            Freelook.Instance.maxY = 10;
        }

        if (currentRotation > 0 && gunName == "Player Gun" && firePressed == false) ;
        {
            textUnderGun.text = "HOLD RMB";
        }

        if(currentRotation <= 0 && gunName == "Player Gun")
        {
            textUnderGun.text = "CLICK LMB";
        }
    }

    private IEnumerator AiFire(GameObject gun)
    {
        AiShot = true;

        int randForBullet = UnityEngine.Random.Range(1, 7);

        AiRandom = randForBullet;

        //AiRandom = 8;
        
        //UnityEngine.Debug.Log("bullet " + GameManager.Instance.bullets);
        //UnityEngine.Debug.Log("random " + AiRandom);

        yield return new WaitForSeconds(1.5f);
        firePressed = true;
        gunAnim.Play("Airecoil");
        if (AiRandom <= GameManager.Instance.bullets)
        {
            Flash.Play();
            SFXManager.instance.PlaySFXClip(Gunfire, transform, 0.3f);
        }
        yield return new WaitForSeconds(delay);

        float chance = gameManager.statusPercent;
        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= chance)
        {
            //Shoots off your own finger
            GameManager.Instance.ReduceHealth(1, 3);
            GameManager.Instance.gunBackfire.gameObject.SetActive(true);
        }
        else if (AiRandom <= GameManager.Instance.bullets)
        {
            GameManager.Instance.CheckArmour(2, 3);
            GameManager.Instance.bullets--;
            if (GameManager.Instance.bullets < 1)
            {
                GameManager.Instance.bullets = 1;
            }
        }

        GameManager.Instance.Gun.SetActive(true);
        GameManager.Instance.aiGunActive = false;
        gun.SetActive(false);

        gameManager.inGunAction = false;

        gameManager.aiGunCount = 0;

        //if other card is gun then wait
        if (GameManager.Instance.has2Guns == false)
        {
            GameManager.Instance.inGunAction = false;
        }

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
        yield return new WaitForSeconds(delay);
        if (rand == 0)
        {
                //Shoots off your own finger
            GameManager.Instance.ReduceHealth(2, 3);
            GameManager.Instance.gunBackfire.gameObject.SetActive(true);
        }
        else if (PRandom <= GameManager.Instance.bullets)
        {
            GameManager.Instance.CheckArmour(1, 3);
            GameManager.Instance.bullets--;
            if(GameManager.Instance.bullets < 1)
            {
                GameManager.Instance.bullets = 1;
            }
        }

        
        Freelook.Instance.minX = -30;
        Freelook.Instance.maxX = 60;
        Freelook.Instance.minY = -75;
        Freelook.Instance.maxY = 75;


        GameManager.Instance.playerGunActive = false;
        GameManager.Instance.Gun.SetActive(true);
        GameManager.Instance.playerGunActive = false;
        gun.SetActive(false);
        if (GameManager.Instance.has2Guns == false)
        {
            GameManager.Instance.inGunAction = false;
            PlayerShot = false;
        }
        gunAnim.Play("GunPause");
        firePressed = false;
    }
}
