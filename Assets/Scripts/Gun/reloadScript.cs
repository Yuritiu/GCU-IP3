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
    private bool isActive = false;
    public int currentBullets = 0;

    public List<GameObject> PlayerBullets;
    public List<GameObject> AiBullets;
    public List<GameObject> TableBullets;

    private GameObject bulletToLoad;

    [SerializeField] private AudioClip chamberSpin;

    private CameraController cameraController;

    public bool reloadHappened;
    public GameObject chamber;

    public bool in1stPos = false;
    public bool in2ndPos = false;

    public bool UsedTwice = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        in1stPos = true;
        //cameraController = FindFirstObjectByType<CameraController>();


    }

    public void moveGun()
    {
        if (in1stPos == true && isActive == false)
        {
            in2ndPos=true;
            in1stPos= false;
            //shootScript.gunAnim.Play("GunPause");

            StartCoroutine(gunTransition(targetPos));
            isActive = true;
        }

        else if (in2ndPos == true)
        {
            in1stPos=true;
            in2ndPos=false;
            //shootScript.gunAnim.Play("GunPause");
            StartCoroutine(gunTransition(gunPos));
            
            isActive = false;
        }

        if(GameManager.Instance.inGunAction == true)
        {
            in1stPos= true;
            
            moveGun();
        }
    }

    void loadGun()
    {
        StartCoroutine(loadWeapon(chamber));
    }

    //Check If Gun Action Is Happening
    private bool GunActionInProgress()
    {
        return GameManager.Instance.inGunAction|| GameManager.Instance.aiGunCount > 0;
    }

    private void Update()
    {
        if (GameManager.Instance.bullets > 0 && GameManager.Instance.bullets <= PlayerBullets.Count)
        {
            PlayerBullets[GameManager.Instance.bullets - 1].SetActive(true);
            AiBullets[GameManager.Instance.bullets - 1].SetActive(true);
        }
    }

    IEnumerator gunTransition(GameObject Target)
    {
        //cameraController.SetCameraToReloadTarget();
        if (in2ndPos == true)
        {
            yield return new WaitForSeconds(4f);
        }
        else if (in2ndPos == false)
        {
            yield return new WaitForSeconds(2f);
        }
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
        
        gun.transform.position = Target.transform.position;
        gun.transform.rotation = Target.transform.rotation;

        if(in2ndPos == true)
        {
            loadGun();
        }
            
        yield return null;    
    }

   

    IEnumerator loadWeapon(GameObject chamber)
    {

        //Wait Until All Knife/Gun Actions Are Finished
        while (GunActionInProgress())
        {
            Debug.Log("Gun action in progress! Waiting...");
            yield return null;
        }
        

        float x = chamber.transform.position.x;
        SFXManager.instance.PlaySFXClip(chamberSpin, transform, 0.3f);
        yield return new WaitForSeconds(0.3f);
        //chamber.transform.position = new Vector3(0.003f, chamber.transform.position.y, chamber.transform.position.z);
        float t = 0.00f;
        bool moveFinished = false;
        chamber.transform.position = new Vector3(x - 0.01f, chamber.transform.position.y, chamber.transform.position.z);

        while (t < 1.0f && moveFinished == false)
        {
            t += Time.deltaTime;
            chamber.transform.Rotate(0f, 0f, -5f, Space.Self);
            

            if (t >= 1.0f)
            {
                moveFinished = true;
            }
            yield return null;
        }
        while(currentBullets < GameManager.Instance.bullets)
        {
            float t2 = 0;
            Vector3 startingpos = chamber.transform.position;

            //TableBullets[GameManager.Instance.bullets - 1].transform.position = new Vector3(TableBullets[GameManager.Instance.bullets - 1].transform.position.x, TableBullets[GameManager.Instance.bullets - 1].transform.position.y + 3f, TableBullets[GameManager.Instance.bullets - 1].transform.position.z + 1f);
            //TableBullets[GameManager.Instance.bullets - 1].SetActive(true);
            if (GameManager.Instance.bullets > 0)
            {
                bulletToLoad = TableBullets[currentBullets];
                startingpos = bulletToLoad.transform.position;
                bulletToLoad.transform.position = new Vector3(chamber.transform.position.x, chamber.transform.position.y + 0.06f, chamber.transform.position.z - 0.06f);
                bulletToLoad.SetActive(true);

                while (t2 < 1.0f)
                {
                    t2 += Time.deltaTime;
                    bulletToLoad.transform.position = Vector3.Lerp(bulletToLoad.transform.position, startingpos, t2);
                    yield return null;

                }
                
            }
            currentBullets = currentBullets +1;
            yield return null;

        }
       
               
        chamber.transform.position = new Vector3(x, chamber.transform.position.y, chamber.transform.position.z);
        isActive = false;

        print("im here");

        moveGun();
        isActive = false;

        //Wait For Gun To Move Back To Table
        yield return new WaitForSeconds(2f);
        //cameraController.SetCameraToOpponentTarget();

        GameManager.Instance.FinishPlayerReload();
        GameManager.Instance.FinishAIReload();

        yield return null;
    }    
}

