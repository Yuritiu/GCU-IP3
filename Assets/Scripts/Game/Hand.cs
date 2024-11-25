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
    private Vector3 knifePos;
    private Quaternion knifeRot;

    private void Start()
    {
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
                //move knife back and forward
                //from -0.15 to 0.1 degrees rotation on the z axis

                turn.x += Input.GetAxis("Mouse X") * sensitivity;

                //print(knife.transform.localRotation.z);
                if (knife.transform.localRotation.z <= 0.15 && knife.transform.localRotation.z >= -0.2)
                {
                    knife.transform.localRotation = Quaternion.Euler(0, 0, -turn.x);
                }
                if(knife.transform.localRotation.z > 0.1)
                {
                    if (sideToHit)
                    {
                        movedKnifeEnough++;
                        sideToHit = false;
                    }
                    knife.transform.localRotation = Quaternion.Euler(0, 0, 8f);
                }
                if (knife.transform.localRotation.z < -0.15)
                {
                    if (!sideToHit)
                    {
                        //print(movedKnifeEnough);
                        movedKnifeEnough++;
                        sideToHit = true;
                    }
                    knife.transform.localRotation = Quaternion.Euler(0, 0, -13f);
                }

                //after knife has moved back and forward several times remove it from the hand
                if (movedKnifeEnough > 10)
                {
                    //print("Remove Finger");
                    EndOfAction(GameManager.Instance.playerFingers);
                }
            }
        }
    }

    public void StartOfAction()
    {
        //print("Start of knife action");
        if (!GameManager.Instance.inGunAction)
        {
            GameManager.Instance.inKnifeAction = true;
            //move knife into finger
            Transform knifeGameObject = knife.gameObject.transform;
            //move camera infront of hand
            GameManager.Instance.in2ndPos = false;
            GameManager.Instance.in3rdPos = true;
            StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target3));
            knifeGameObject.SetPositionAndRotation(fingers[GameManager.Instance.playerFingers].gameObject.transform.position, Quaternion.Euler(0, -150, 0));
        }
        else
        {
            StartCoroutine(WaitToStart());
        }
    }

    private void EndOfAction(int num)
    {
        knife.gameObject.transform.SetPositionAndRotation(knifePos,knifeRot);

        GameManager.Instance.playerFingers--;
        CheckForSecondAction();
        RemoveFinger(num);
    }

    public void CheckForSecondAction()
    {
        GameManager.Instance.numberOfKnifeCards--;
        //print(GameManager.Instance.numberOfKnifeCards);
        if (GameManager.Instance.numberOfKnifeCards >= 1)
        {
            StartOfAction();
            GameManager.Instance.numberOfKnifeCards = 0;
        }
        else
        {
            GameManager.Instance.knife1used = false;
            GameManager.Instance.knife2used = false;
            GameManager.Instance.inKnifeAction = false;
            GameManager.Instance.canCutFinger = false;
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

    private void DisableCamera()
    {
        GameManager.Instance.in3rdPos = false;
        StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target1));
    }
}
