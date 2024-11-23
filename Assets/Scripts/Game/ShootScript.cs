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

        gunAnim = GetComponent<Animator>();

        if (gameObject.name == "Ai Gun")
        {
            
            StartCoroutine(AiFire(gameObject));
            
        }
    }
    private void Update()
    {


      


        gunAnim = GetComponent<Animator>();
        if (gameObject.name == "Player Gun" && Input.GetMouseButtonDown(0) && firePressed == false)
        {
            
            StartCoroutine(Fire(gameObject));
            
        }

       

    }


    private IEnumerator AiFire(GameObject gun)
    {
        yield return new WaitForSeconds(1.5f);
        firePressed = true;
        gunAnim.Play("Airecoil");
        if (AiRandom == 1)
        {
            Flash.Play();
        }
        yield return new WaitForSeconds(delay);

        if(AiRandom == 1)
        {
            GameManager.Instance.EndGameLose();
        }

        GameManager.Instance.Gun.SetActive(true);
        gun.SetActive(false);

        GameManager.Instance.inGunAction = false;
        gunAnim.Play("GunPause");
        firePressed = false;
        AiShot = false;
        
        
    }


    private IEnumerator Fire(GameObject gun)
    {
        
        firePressed = true;
        gunAnim.Play("recoil");
        if (PRandom == 2)
        {
            Flash.Play();
        }
        yield return new WaitForSeconds(delay);
        if(PRandom == 2)
        {
            GameManager.Instance.EndGameWin();
        }

        GameManager.Instance.playerGunActive = false;
        GameManager.Instance.Gun.SetActive(true);
        gun.SetActive(false);
        GameManager.Instance.inGunAction = false;
        gunAnim.Play("GunPause");
        firePressed = false;
        PlayerShot = false;

        

        
    }
}
