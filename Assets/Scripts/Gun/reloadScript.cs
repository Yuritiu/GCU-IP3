using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class reloadScript : MonoBehaviour
{
    public static reloadScript Instance;
    public Animator gunAnim;
    [HideInInspector] public GameObject gun;
    public bool reloadHappened;
    public GameObject chamber;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        gun = this.gameObject;
        
        reloadHappened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (reloadHappened == false)
        {
           
            StartCoroutine(reload());

        }

        
    }

    private void moveGunForReload()
    {
        //gunAnim.Play("GunPause");
        
    }

    public IEnumerator reload()
    {
        gunAnim.Play("load");
        yield return new WaitForSeconds(1f) ;
        reloadHappened = true;
        gunAnim.Play("GunPause");
    }    
}
