using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
    [SerializeField] List<GameObject> fingers;
    [SerializeField] GameObject knife;
    private int movedKnifeEnough = 0;
    public Vector2 turn;
    public float sensitivity = .5f;
    public bool sideToHit = false;
    public bool waitingToCut = false;
    
    private Vector3 knifePos;
    private Quaternion knifeRot;

    [SerializeField] private GameObject actionUI;

    [SerializeField] private AudioClip PlayerScream;
    [SerializeField] private AudioClip[] Cutting;

    [Header("References")]
    [SerializeField] ParticleSystem bloodParticleSystem1;
    [SerializeField] ParticleSystem bloodParticleSystem2;
    [SerializeField] ParticleSystem bloodParticleSystem3;
    [SerializeField] ParticleSystem bloodParticleSystem4;
    [SerializeField] ParticleSystem bloodParticleSystem5;

    private void Start()
    {
        //THIS IS REALLY BAD!!
        bloodParticleSystem1 = GameObject.FindGameObjectWithTag("BloodParticles1").GetComponent<ParticleSystem>();
        bloodParticleSystem2 = GameObject.FindGameObjectWithTag("BloodParticles2").GetComponent<ParticleSystem>();
        bloodParticleSystem3 = GameObject.FindGameObjectWithTag("BloodParticles3").GetComponent<ParticleSystem>();
        bloodParticleSystem4 = GameObject.FindGameObjectWithTag("BloodParticles4").GetComponent<ParticleSystem>();
        bloodParticleSystem5 = GameObject.FindGameObjectWithTag("BloodParticles5").GetComponent<ParticleSystem>();

        bloodParticleSystem1.Stop();
        bloodParticleSystem2.Stop();
        bloodParticleSystem3.Stop();
        bloodParticleSystem4.Stop();
        bloodParticleSystem5.Stop();

        if (this.gameObject.tag == "Player")
        {
            knifePos = knife.gameObject.transform.position;
            knifeRot.eulerAngles = knife.gameObject.transform.eulerAngles;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.inGunAction)
        {
            return;
        }
        //print(GameManager.Instance.playerFingers);
        else if (this.gameObject.tag == "Player")
        {
            if (GameManager.Instance.canCutFinger)
            {
                //print(turn.x);
                if (!waitingToCut)
                {
                    //move knife back and forward
                    //from -0.15 to 0.1 degrees rotation on the z axis
                    turn.x += Input.GetAxis("Mouse X") * sensitivity;

                    //print(turn.x);
                    if (-turn.x <= 18 && -turn.x >= -13)
                    {
                        knife.transform.localRotation = Quaternion.Euler(0, 0, -turn.x);
                    }
                }

                if(turn.x > 18)
                {
                    //print("too big");
                    if (sideToHit)
                    {
                        waitingToCut = true;
                        StartCoroutine(WaitToCut());
                        movedKnifeEnough++;
                        sideToHit = false;
                        SFXManager.instance.PlayRandomSFXClip(Cutting, transform, 0.2f);
                    }
                    turn.x = 18;
                }
                if (turn.x < -13)
                {
                    //print("too small");
                    if (!sideToHit)
                    {
                        //print(movedKnifeEnough);
                        waitingToCut = true;
                        StartCoroutine(WaitToCut());
                        movedKnifeEnough++;
                        sideToHit = true;
                        SFXManager.instance.PlayRandomSFXClip(Cutting, transform, 0.2f);
                    }
                    turn.x = -13;
                }

                //after knife has moved back and forward several times remove it from the hand
                if (movedKnifeEnough > 3)
                {
                    //print("Remove Finger");
                    EndOfAction(GameManager.Instance.playerFingers);
                    SFXManager.instance.PlaySFXClip(PlayerScream, transform, 0.2f);
                }
            }
        }
    }

    public void StartOfAction()
    {
        //print(GameManager.Instance.inGunAction);
        if (!GameManager.Instance.inGunAction)
        {
            waitingToCut = false;
            GameManager.Instance.inKnifeActionAiPlayed = true;
            //move knife into finger
            Transform knifeGameObject = knife.gameObject.transform;
            actionUI.SetActive(true);
            //move camera infront of hand
            GameManager.Instance.in2ndPos = false;
            GameManager.Instance.in3rdPos = true;
            GameManager.Instance.cameraMovement = false; //disables W S P Camera controls
            StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target3));
            knifeGameObject.SetPositionAndRotation(fingers[GameManager.Instance.playerFingers].gameObject.transform.position, Quaternion.Euler(0, -150, 0));
            GameManager.Instance.canCutFinger = true;
        }
        else
        {
            StartCoroutine(WaitToStart());
        }
    }

    private void EndOfAction(int num)
    {
        knife.gameObject.transform.SetPositionAndRotation(knifePos,knifeRot);
        actionUI.SetActive(false);

        GameManager.Instance.playerFingers--;
        CheckForSecondAction();
        RemoveFinger(num);
    }

    public void CheckForSecondAction()
    {
        //print(GameManager.Instance.numberOfKnifeCards);
        GameManager.Instance.numberOfKnifeCards--;
        //print(GameManager.Instance.numberOfKnifeCards);
        if (GameManager.Instance.numberOfKnifeCards >= 1 && GameManager.Instance.playerFingers > 0)
        {
            StartOfAction();
            GameManager.Instance.numberOfKnifeCards = 0;
        }
        else
        {
            GameManager.Instance.knife1used = false;
            GameManager.Instance.knife2used = false;
            GameManager.Instance.aiHasKnife = false;
            GameManager.Instance.inKnifeActionAiPlayed = false;
            GameManager.Instance.canCutFinger= false;
            GameManager.Instance.numberOfKnifeCards = 0;
            DisableCamera();
        }
    }
    
    public void RemoveFinger(int num)
    {
        movedKnifeEnough = 0;
        Destroy(fingers[num]);
        fingers.Remove(fingers[num]);

        if(num == 1)
        {
            bloodParticleSystem1.Play();
        }
        else if(num == 2)
        {
            bloodParticleSystem2.Play();
        }
        else if(num == 3)
        {
            bloodParticleSystem3.Play();
        }
        else if (num == 4)
        {
            bloodParticleSystem4.Play();
        }
        else if (num == 5)
        {
            bloodParticleSystem5.Play();
        }
        GameManager.Instance.CheckFingers();
    }

    IEnumerator WaitToStart()
    {
        //waits for cards to reveal
        yield return new WaitForSeconds(1f);
        StartOfAction();
    }
    
    IEnumerator WaitToCut()
    {
        //waits to contiune cutting to add more tension
        yield return new WaitForSeconds(1f);
        waitingToCut = false;
    }

    private void DisableCamera()
    {
        StartCoroutine(DisableCameraWithDelay());
    }

    private IEnumerator DisableCameraWithDelay()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.cameraMovement = true;
        GameManager.Instance.in3rdPos = false;
        StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target1));
    }

}
