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




    private void OnEnable()
    {
        gameObject.transform.position = Target1.position;

        if(gameObject.name == "Ai Gun")
        {
            StartCoroutine(DestroyAiGun(gameObject));
        }
    }
    void Update()
    {
        if(gameObject.activeInHierarchy && In2ndPos == false)
        {
            StartCoroutine(MoveGun(Target2));
            In2ndPos = true;
        }

        if (gameObject.name == "Player Gun" && Input.GetKey("f"))
        {
            gameObject.SetActive(false);
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

        }



        yield return 0;
    }



    private IEnumerator DestroyAiGun(GameObject Gun)
    {


        yield return new WaitForSeconds(6);
        Gun.SetActive(false);
        
    }
}
