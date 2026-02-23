using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerHand : NetworkBehaviour
{
    [Header("Hand & Finger Models")]
    [SerializeField] private List<GameObject> handModels;
    [SerializeField] private List<GameObject> fingerModels;
    private int currentHandIndex = 0;

    [Header("Hand Cards")]
    public List<CardType> PlayerHandCards = new List<CardType>();

    [Header("Knife & Arm")]
    [SerializeField] private GameObject knife;
    [SerializeField] private GameObject arm;

    [Header("Knife Settings")]
    [SerializeField] private float sensitivity = 0.5f;
    private Vector2 turn;
    private int knifeSwings = 0;
    private Vector3 knifeDefaultPos;
    private Quaternion knifeDefaultRot;

    [Header("UI & Effects")]
    [SerializeField] private GameObject actionUI;
    [SerializeField] private ParticleSystem[] bloodParticles;
    [SerializeField] private AudioClip knifeInsertSFX;
    [SerializeField] private AudioClip[] cuttingSFX;
    [SerializeField] private AudioClip[] screamSFX;

    [Header("Camera Reference")]
    private MultiplayerCamera cameraController;

    private bool waitingToCut = false;
    private bool bloodSplatterActive = false;
    private int bloodSplatterIndex = 0;
    private int phaseOfAction = 1;
    private bool sideToHit = false;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false; //Only local player controls the hand
            return;
        }

        knifeDefaultPos = knife.transform.position;
        knifeDefaultRot = knife.transform.rotation;

        cameraController = FindFirstObjectByType<MultiplayerCamera>();
        turn.y = 9;

        //Stop all particle systems initially
        foreach (var ps in bloodParticles)
        {
            ps.Stop();
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!GameManager.Instance.canCutFinger) return;

        if (Input.GetMouseButton(0))
        {
            HandleKnifeAction();
        }
    }

    public void SetCards(List<CardType> cards)
    {
        PlayerHandCards = new List<CardType>(cards);
    }

    private void HandleKnifeAction()
    {
        //Phase 1: Move knife up and down
        if (phaseOfAction == 1 && !waitingToCut)
        {
            turn.y += Input.GetAxis("Mouse Y") * sensitivity;
            turn.y = Mathf.Clamp(turn.y, 7.5f, 9f);
            knife.transform.position = new Vector3(knife.transform.position.x, turn.y / 10f, knife.transform.position.z);

            if (turn.y <= 7.5f)
            {
                phaseOfAction = 2;
                bloodSplatterIndex = Random.Range(0, bloodParticles.Length);
                bloodSplatterActive = true;

                SFXManager.instance.PlaySFXClip(knifeInsertSFX, transform, 1f);
            }
        }
        //Phase 2: Move knife back and forth
        else if (phaseOfAction == 2 && !waitingToCut)
        {
            turn.x += Input.GetAxis("Mouse X") * (sensitivity - 0.2f);
            turn.x = Mathf.Clamp(turn.x, -5f, 7f);

            knife.transform.localRotation = Quaternion.Euler(0, 0, -turn.x);
            knife.transform.position = new Vector3(
                fingerModels[GameManager.Instance.playerFingers].transform.position.x + (turn.x / 1000f),
                knife.transform.position.y,
                knife.transform.position.z
            );

            if ((sideToHit && turn.x > 7f) || (!sideToHit && turn.x < -5f))
            {
                knifeSwings++;
                sideToHit = !sideToHit;
                SFXManager.instance.PlayRandomSFXClip(cuttingSFX, transform, 0.2f);
            }

            if (knifeSwings > 9)
            {
                EndKnifeAction(GameManager.Instance.playerFingers);
            }
        }
    }

    public void StartKnifeAction()
    {
        if (!IsOwner) return;

        waitingToCut = false;
        phaseOfAction = 1;
        GameManager.Instance.inKnifeActionAiPlayed = true;
        actionUI.SetActive(true);

        //Move knife to the target finger
        Transform targetFinger = fingerModels[GameManager.Instance.playerFingers].transform;
        knife.transform.SetPositionAndRotation(targetFinger.position + Vector3.up * 0.1f, Quaternion.identity);

        if (arm != null) arm.SetActive(true);
        GameManager.Instance.cameraMovement = false;
        cameraController.SetCameraToKnifeTarget();
    }

    private void EndKnifeAction(int fingerIndex)
    {
        knifeSwings = 0;
        knife.transform.SetPositionAndRotation(knifeDefaultPos, knifeDefaultRot);

        if (arm != null) arm.SetActive(false);
        actionUI.SetActive(false);

        GameManager.Instance.playerFingers--;
        SFXManager.instance.PlayRandomSFXClip(screamSFX, transform, 0.15f);

        RemoveFingerModel(fingerIndex);
        StartCoroutine(CheckForSecondAction());
    }

    private void RemoveFingerModel(int index)
    {
        if (currentHandIndex < handModels.Count - 1)
        {
            fingerModels[currentHandIndex].SetActive(true);
            handModels[currentHandIndex].SetActive(false);
            currentHandIndex++;
            handModels[currentHandIndex].SetActive(true);
        }

        GameManager.Instance.CheckFingers();
    }

    private IEnumerator CheckForSecondAction()
    {
        yield return new WaitForSeconds(1f);

        bool secondActionNeeded = false;

        if (IsOwner)
        {
            if (GameManager.Instance.CheckKnifeAI() && GameManager.Instance.playerFingers > 0)
                secondActionNeeded = true;
        }
        else
        {
            if (GameManager.Instance.CheckKnifePlayer() && GameManager.Instance.playerFingers > 0)
                secondActionNeeded = true;
        }

        if (secondActionNeeded)
        {
            StartKnifeAction();
        }
        else
        {
            GameManager.Instance.inKnifeActionAiPlayed = false;
            GameManager.Instance.canCutFinger = false;
            cameraController.SetCameraToOpponentTarget();
            cameraController.cameraLocked = false;
            GameManager.Instance.cameraMovement = true;
        }
    }

    public void DisableCameraAfterAction()
    {
        if (!IsOwner || cameraController == null) return;

        //Reenable player movement
        GameManager.Instance.cameraMovement = true;

        //Unlock and reset camera
        cameraController.cameraLocked = false;
        cameraController.SetCameraToOpponentTarget();
    }
}