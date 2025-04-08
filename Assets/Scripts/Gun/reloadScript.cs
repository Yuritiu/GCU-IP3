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
    [SerializeField] private AudioClip bulletLoad;

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
        cameraController = FindFirstObjectByType<CameraController>();
    }

    public void moveGun()
    {
        if (in1stPos == true && isActive == false)
        {
            in2ndPos = true;
            in1stPos = false;
            //shootScript.gunAnim.Play("GunPause");

            StartCoroutine(FinishReloads());
            StartCoroutine(gunTransition(targetPos));
            isActive = true;
        }

        else if (in2ndPos == true)
        {
            in1stPos = true;
            in2ndPos = false;
            //shootScript.gunAnim.Play("GunPause");
            StartCoroutine(gunTransition(gunPos));

            isActive = false;
        }

        if (GameManager.Instance.inGunAction == true)
        {
            in1stPos = true;

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
        return GameManager.Instance.inGunAction || GameManager.Instance.aiGunCount > 0;
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

        while (t < 1.0f && moveFinished == false)
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

        if (in2ndPos == true)
        {
            loadGun();
        }

        yield return null;
    }

    IEnumerator loadWeapon(GameObject chamber)
    {
        float speedMultiplier = 1.5f; //Action speed multiplier (as it's done by time.deltatime needed to make one)

        //Waits till other actions done
        while (GunActionInProgress())
        {
            Debug.Log("Gun action in progress! Waiting...");
            yield return null;
        }

        cameraController.SetCameraToReloadTarget();

        float x = chamber.transform.position.x;

        yield return new WaitForSeconds(1f);

        //Step 1: Hinge out the chamber
        Debug.Log("Hinging chamber out...");
        SFXManager.instance.PlaySFXClip(chamberSpin, transform, 0.3f);

        float hingeTime = 0f;
        float yDelay = 0.2f * speedMultiplier;
        Quaternion initialRotation = chamber.transform.localRotation;
        Vector3 initialChamberPosition = chamber.transform.position;

        float spinOutTime = 0f;
        bool spinOutFinished = false;

        Vector3 initialPosition = chamber.transform.position;
        Vector3 targetPosition = new Vector3(x - 0.03f, chamber.transform.position.y - 0.02f, chamber.transform.position.z);

        while (hingeTime < 1.0f)
        {
            if (!spinOutFinished)
            {
                spinOutTime += Time.deltaTime * speedMultiplier;
                chamber.transform.Rotate(0f, 0f, +10f, Space.Self);

                if (spinOutTime >= 1.0f)
                {
                    spinOutFinished = true;
                }
            }

            hingeTime += Time.deltaTime * speedMultiplier;

            //Start X movement 
            float newX = Mathf.Lerp(initialPosition.x, targetPosition.x, hingeTime);

            //Start Y movement with a slight delay
            float newY = Mathf.Lerp(initialPosition.y, targetPosition.y, Mathf.Max(0, hingeTime - yDelay));

            chamber.transform.position = new Vector3(newX, newY, chamber.transform.position.z);

            yield return null;
        }


        yield return new WaitForSeconds(1f);

        //Step 2: Load the bullets
        Debug.Log("Loading bullets...");
        while (currentBullets < GameManager.Instance.bullets)
        {
            float t2 = 0;
            Vector3 startingPos = chamber.transform.position;

            if (GameManager.Instance.bullets > 0)
            {
                bulletToLoad = TableBullets[currentBullets];
                startingPos = bulletToLoad.transform.position;
                bulletToLoad.transform.position = new Vector3(chamber.transform.position.x, chamber.transform.position.y + 0.06f, chamber.transform.position.z - 0.06f);
                bulletToLoad.SetActive(true);
                SFXManager.instance.PlaySFXClip(bulletLoad, transform, 0.7f);

                while (t2 < 1.0f)
                {
                    t2 += Time.deltaTime * speedMultiplier;
                    bulletToLoad.transform.position = Vector3.Lerp(bulletToLoad.transform.position, startingPos, t2);
                    yield return null;
                }
            }

            currentBullets += 1;
            yield return new WaitForSeconds(0.1f); //Bullet load delay time
        }


        //Step 3: Spin and close at the same time
        Debug.Log("Spinning and closing chamber...");
        SFXManager.instance.PlaySFXClip(chamberSpin, transform, 0.3f);

        float spinTime = 0f;
        float closeTime = 0f;
        bool spinFinished = false;
        bool closeFinished = false;

        while (!spinFinished || !closeFinished)
        {
            if (!spinFinished)
            {
                spinTime += Time.deltaTime * speedMultiplier;
                chamber.transform.Rotate(0f, 0f, -10f, Space.Self);

                if (spinTime >= 1.0f)
                {
                    spinFinished = true;
                }
            }

            if (!closeFinished)
            {
                closeTime += Time.deltaTime * speedMultiplier;

                float newX = Mathf.Lerp(x - 0.03f, x, closeTime);
                float newY = Mathf.Lerp(chamber.transform.position.y, initialChamberPosition.y, closeTime);

                chamber.transform.position = new Vector3(newX, newY, chamber.transform.position.z);

                if (closeTime >= 1.0f)
                {
                    closeFinished = true;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Step 5: Gun spin
        Debug.Log("Doing Gun Spin...");
        float revolverSpinTime = 0f;

        while (revolverSpinTime < 0.5f)
        {
            revolverSpinTime += Time.deltaTime;

            // Rotate based on the total elapsed time
            float rotationAmount = -360f * (Time.deltaTime / 0.5f);
            gun.transform.Rotate(rotationAmount, 0f, 0f, Space.Self);

            yield return null;
        }

        //Step 4: Put down the gun
        Debug.Log("Putting down the gun...");
        isActive = false;
        moveGun();

        //Wait for gun to move back to table
        yield return new WaitForSeconds(1f);
        cameraController.SetCameraToOpponentTarget();

        GameManager.Instance.FinishPlayerReload();
        GameManager.Instance.FinishAIReload();
    }
    IEnumerator FinishReloads()
    {
        yield return new WaitForSeconds(9f);

        GameManager.Instance.FinishPlayerReload();
        GameManager.Instance.FinishAIReload();

        yield return null;
    }
}

