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
    

    private void Update()
    {
        if (this.gameObject.tag == "Player")
        {
            if (GameManager.Instance.inKnifeAction)
            {
                //move knife back and forward
                //from -0.15 to 0.1 degrees rotation on the z axis

                turn.x += Input.GetAxis("Mouse X") * sensitivity;

                print(knife.transform.localRotation.z);
                if (knife.transform.localRotation.z <= 0.1 && knife.transform.localRotation.z >= -0.15)
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
                    knife.transform.localRotation = Quaternion.Euler(0, 0, 10f);
                }
                if (knife.transform.localRotation.z < -0.15)
                {
                    if (!sideToHit)
                    {
                        movedKnifeEnough++;
                        sideToHit = true;
                    }
                    knife.transform.localRotation = Quaternion.Euler(0, 0, -15f);
                }

                //after knife has moved back and forward several times remove it from the hand
                if (movedKnifeEnough > 20)
                {
                    print("Remove Finger");
                    RemoveFinger(GameManager.Instance.playerFingers);
                }
            }
        }
    }

    public void StartOfAction()
    {
        //move camera infront of hand
        GameManager.Instance.in2ndPos = false;
        GameManager.Instance.in3rdPos = true;
        StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target3));

        //move knife into finger
        Transform knifeGameObject = knife.gameObject.transform;
        knifeGameObject.SetPositionAndRotation(fingers[GameManager.Instance.playerFingers].gameObject.transform.position, Quaternion.Euler(0, -150, 0));
    }


    public void RemoveFinger(int num)
    {
        StartCoroutine(GameManager.Instance.CameraTransitionIEnum(GameManager.Instance.Target1));
        movedKnifeEnough = 0;
        GameManager.Instance.inKnifeAction = false;
        Destroy(fingers[num]);
        fingers.Remove(fingers[num]);
    }
}
