using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    

    private GameManager gameManager;
    [SerializeField] private AudioClip Gunload;
    [SerializeField] private AudioClip Gunfire;


    private void Start()
    {
        gunAnim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        
    }

    private void Awake()
    {
        if (gunName == "Ai Gun")
        {
            //print("instanceed1");
            instance1 = this;
            SFXManager.instance.PlaySFXClip(Gunload, transform, 1f);
        }
        else
        {
            instance2 = this;
            SFXManager.instance.PlaySFXClip(Gunload, transform, 1f);
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
        if (gunName == "Player Gun" && Input.GetMouseButtonDown(0) && firePressed == false)
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(Fire(gameObject));
            
        }


        
    }


    private IEnumerator AiFire(GameObject gun)
    {
        if (this != null)
        {
            AiShot = true;

            int randForBullet = UnityEngine.Random.Range(1, 7);

            AiRandom = randForBullet;

            //UnityEngine.Debug.Log("bullet " + GameManager.Instance.bullets);
            //UnityEngine.Debug.Log("random " + AiRandom);

            yield return new WaitForSeconds(1.5f);
            firePressed = true;
            gunAnim.Play("Airecoil");
            if (AiRandom <= GameManager.Instance.bullets)
            {
                Flash.Play();
                SFXManager.instance.PlaySFXClip(Gunfire, transform, 1f);
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

            //if other card is gun then wait
            if (GameManager.Instance.has2Guns == false)
            {
                GameManager.Instance.inGunAction = false;
            }

            gunAnim.Play("GunPause");
            firePressed = false;
            AiShot = false;
        }
    }

    private IEnumerator Fire(GameObject gun)
    {
        //print("before null check");
        if (this != null)
        {
            PlayerShot = true;

            //print("not null");
            int rand = UnityEngine.Random.Range(0, 5);
            
            int randForBullet = UnityEngine.Random.Range(1, 7);

            PRandom = randForBullet;

            firePressed = true;
            gunAnim.Play("recoil");
            if (PRandom <= GameManager.Instance.bullets)
            {
                Flash.Play();
                SFXManager.instance.PlaySFXClip(Gunfire, transform, 1f);
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
}
