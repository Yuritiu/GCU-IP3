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
            StartCoroutine(gunTransition(targetPos));
            in2ndPos=true;
            in1stPos= false;
        }

        else if (in2ndPos == true)
        {
            StartCoroutine(gunTransition(gunPos));
            in1stPos=true;
            in2ndPos=false;
        }
    }
    

    
    IEnumerator gunTransition(GameObject Target)
    {
        float t = 0.00f;
        Vector3 startingpos = gameObject.transform.position;

        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale * speed);
            gameObject.transform.position = Vector3.Lerp(startingpos, Target.transform.position, t);

            yield return null;
        }

        
    }

    
}
