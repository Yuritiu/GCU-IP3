using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
    public static Hand Instance;

    [SerializeField] List<GameObject> fingers;
    [SerializeField] GameObject knife;
    private int movedKnifeEnough = 0;
    public Vector2 turn;
    public float sensitivity = .5f;
    public bool sideToHit = false;
    public bool waitingToCut = false;
    public int phaseOfAction = 1;
    
    private Vector3 knifeDefaultPos;
    private Quaternion knifeRot;

    [SerializeField] private GameObject actionUI;

    [SerializeField] private AudioClip[] Cutting;
    [SerializeField] private AudioClip[] playerScreams;
    [SerializeField] private AudioClip knifeInsert;

    [Header("References")]
    [SerializeField] public ParticleSystem bloodParticleSystem1;
    [SerializeField] public ParticleSystem bloodParticleSystem2;
    [SerializeField] public ParticleSystem bloodParticleSystem3;
    [SerializeField] public ParticleSystem bloodParticleSystem4;
    [SerializeField] public ParticleSystem bloodParticleSystem5;

    private void Awake()
    {
        Instance = this;
    }

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
            knifeDefaultPos = knife.gameObject.transform.position;
            knifeRot.eulerAngles = knife.gameObject.transform.eulerAngles;
        }

        turn.y = 9;
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
                if (phaseOfAction == 1)
                {
                    if (!waitingToCut)
                    {
                        //move knife up and down
                        turn.y += Input.GetAxis("Mouse Y") * sensitivity;

                        //print(turn.y);

                        if (turn.y >= 7.5 && turn.y <= 10)
                        {
                            knife.transform.position = new Vector3(knife.transform.position.x, turn.y / 10, knife.transform.position.z);
                        }
                        else if (turn.y >= 10)
                        {
                            turn.y = 10;
                        }
                        else if (turn.y <= 7.5)
                        {
                            turn.y = 7.5f;
                            knife.transform.position = new Vector3(knife.transform.position.x , turn.y/10, knife.transform.position.z);
                            phaseOfAction = 2;
                            SFXManager.instance.PlaySFXClip(knifeInsert, transform, 1f);

                        }
                    }
                }

                if (phaseOfAction == 2)
                {
                    //print(turn.x);
                    if (!waitingToCut)
                    {
                        //move knife back and forward
                        //from -0.15 to 0.1 degrees rotation on the z axis
                        turn.x += Input.GetAxis("Mouse X") * (sensitivity -0.2f);

                        //print(turn.x);
                        if (-turn.x <= 18 && -turn.x >= -13)
                        {
                            knife.transform.localRotation = Quaternion.Euler(0, 0, -turn.x); 
                            knife.transform.position = new Vector3(fingers[GameManager.Instance.playerFingers].gameObject.transform.position.x + (turn.x/1000), knife.transform.position.y , knife.transform.position.z);
                        }
                    }

                    if(turn.x > 7)
                    {
                        //print("too big");
                        if (sideToHit)
                        {
                            //waitingToCut = true;
                            //StartCoroutine(WaitToCut());
                            movedKnifeEnough++;
                            sideToHit = false;
                            SFXManager.instance.PlayRandomSFXClip(Cutting, transform, 0.2f);
                        }
                        turn.x = 7;
                    }
                    if (turn.x < -5)
                    {
                        //print("too small");
                        if (!sideToHit)
                        {
                            //print(movedKnifeEnough);
                            //waitingToCut = true;
                            //StartCoroutine(WaitToCut());
                            movedKnifeEnough++;
                            sideToHit = true;
                            SFXManager.instance.PlayRandomSFXClip(Cutting, transform, 0.2f);
                        }
                        turn.x = -5;
                    }

                    //after knife has moved back and forward several times remove it from the hand
                    if (movedKnifeEnough > 9)
                    {
                        //print("Remove Finger");
                        EndOfAction(GameManager.Instance.playerFingers);
                    }
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
            phaseOfAction = 1;
            GameManager.Instance.inKnifeActionAiPlayed = true;
            //move knife into finger
            Transform knifeGameObject = knife.gameObject.transform;
            actionUI.SetActive(true);
            //move camera infront of hand
            GameManager.Instance.in2ndPos = false;
            GameManager.Instance.in3rdPos = true;
            GameManager.Instance.cameraMovement = false; //disables W S P Camera controls
            StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target3));
            
            turn.y = 9;

            //LERP needed
            knifeGameObject.SetPositionAndRotation(fingers[GameManager.Instance.playerFingers].gameObject.transform.position, Quaternion.Euler(0, 0, 0));
            knifeGameObject.SetPositionAndRotation(new Vector3(knifeGameObject.transform.position.x, knifeGameObject.transform.position.y + 0.1f, knifeGameObject.transform.position.z), Quaternion.Euler(0, 0, 0));
            GameManager.Instance.canCutFinger = true;
        }
        else
        {
            StartCoroutine(WaitToStart());
        }
    }

    private void EndOfAction(int num)
    {
        knife.gameObject.transform.SetPositionAndRotation(knifeDefaultPos,knifeRot);
        actionUI.SetActive(false);

        GameManager.Instance.playerFingers--;
        SFXManager.instance.PlayRandomSFXClip(playerScreams, transform, 0.15f);
        StartCoroutine(CheckForSecondAction());
        RemoveFinger(num);
    }

    public IEnumerator CheckForSecondAction()
    {

        yield return new WaitForSeconds(1f);

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
