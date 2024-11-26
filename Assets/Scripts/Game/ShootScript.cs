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
    

    private void Start()
    {
        gunAnim = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (gameObject.name == "Ai Gun")
        {
            instance1 = this;
        }

        if(gameObject.name == "Player Gun")
        {
            instance2 = this;
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
        if (gameObject.name == "Player Gun" && Input.GetMouseButtonDown(0) && firePressed == false)
        {
            gunAnim = GetComponent<Animator>();
            StartCoroutine(Fire(gameObject));
        }
    }


    private IEnumerator AiFire(GameObject gun)
    {
        if (this != null)
        {

            int rand = UnityEngine.Random.Range(0, 5);
            //\/Debuging\/
            //rand = 0;
            //print(rand);
           
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
            }
            yield return new WaitForSeconds(delay);
            if (rand == 0)
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
        if (this != null)
        {

            int rand = UnityEngine.Random.Range(0, 5);
            //\/Debuging\/
            //rand = 0;
            //print(rand);
            
            int randForBullet = UnityEngine.Random.Range(1, 7);

            PRandom = randForBullet;

            firePressed = true;
            gunAnim.Play("recoil");
            if (PRandom <= GameManager.Instance.bullets)
            {
                Flash.Play();
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
            }
            gunAnim.Play("GunPause");
            firePressed = false;
            PlayerShot = false;
        }
    }
}
