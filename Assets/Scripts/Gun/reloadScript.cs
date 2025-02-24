using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class reloadScript : MonoBehaviour
{
    public GameObject gunPos;
    public GameObject targetPos;
    public static reloadScript Instance;
    public Animator gunAnim;
    [SerializeField] public float speed;
    public GameObject gun;
    [SerializeField] ShootScript shootScript;

    public bool reloadHappened;
    public GameObject chamber;

    public bool in1stPos = false;
    public bool in2ndPos = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        in1stPos = true;
    }

    public void moveGun()
    {
        if (in1stPos == true)
        {
            in2ndPos=true;
            in1stPos= false;
            //shootScript.gunAnim.Play("GunPause");
            StartCoroutine(gunTransition(targetPos));

            
        }

        else if (in2ndPos == true)
        {
            in1stPos=true;
            in2ndPos=false;
            //shootScript.gunAnim.Play("GunPause");
            StartCoroutine(gunTransition(gunPos));
        }
    }
    

    
    IEnumerator gunTransition(GameObject Target)
    {
        float t = 0.00f;
        Vector3 startingpos = gun.transform.position;
        bool moveFinished = false;

        while (t < 1.0f && moveFinished == false )
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            gun.transform.position = Vector3.Lerp(startingpos, Target.transform.position, t);
            gun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, Target.transform.rotation, speed * Time.deltaTime);

            if (t >= 1.0f)
                moveFinished = true;

            yield return null;
        }
        

        
    }

    
}
