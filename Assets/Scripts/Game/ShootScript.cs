using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootScript : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public Transform Target1;
    [SerializeField] public Transform Target2;
    [HideInInspector] public bool In2ndPos;
    public float delay;
    bool firePressed;
    public Animator gunAnim;

    private void Start()
    {
        gunAnim = gameObject.GetComponent<Animator>();
    }


    private void OnEnable()
    {

        
        In2ndPos = false;

        if(gameObject.name == "Ai Gun")
        {
            StartCoroutine(DestroyAiGun(gameObject));
        }
    }
    private void Update()
    {
        if(gameObject.activeInHierarchy == true && In2ndPos == false)
        {
            StartCoroutine(MoveGun(Target2));
            In2ndPos = true;
        }

        gameObject.transform.position = Target2.position;
        if (gameObject.name == "Ai Gun" && firePressed == false)
        {
            StartCoroutine(Fire());
        }

        if (gameObject.name == "Player Gun" && Input.GetMouseButtonDown(0) && firePressed == false)
        {
            StartCoroutine(Fire());
            In2ndPos = false;
            
        }

    }

    private void OnDisable()
    {
        In2ndPos = false;
    }


    private IEnumerator MoveGun(Transform Target)
    {
        float t = 0.00f;
        Vector3 startingPos = gameObject.transform.position;

        while (t < 1.00f)
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            gameObject.transform.position = Vector3.Lerp(startingPos, Target.position, t);
            yield return 0;
        }

        yield return 0;
    }



    private IEnumerator DestroyAiGun(GameObject Gun)
    {
        yield return new WaitForSeconds(6);
        Gun.SetActive(false);
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(1.5f);
        firePressed = true;
        gunAnim.Play("recoil");
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        GameManager.Instance.inGunAction = false;
        gunAnim.Play("GunPause");
        firePressed = false;
    }
}
