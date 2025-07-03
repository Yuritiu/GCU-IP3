using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardVisual : NetworkBehaviour
{
    public NetworkVariable<float> ownerZRotation = new NetworkVariable<float>(180f);

    public float flipDuration = 0.5f;
    private Coroutine flipCoroutine;

    private bool isFaceUp = false;

    private void OnEnable()
    {
        ownerZRotation.OnValueChanged += OnZRotationChanged;
    }

    private void OnDisable()
    {
        ownerZRotation.OnValueChanged -= OnZRotationChanged;
    }

    private void OnZRotationChanged(float oldValue, float newValue)
    {
        //Update Rotation Only if Not Face up
        if (!isFaceUp)
            UpdateCardRotationInstant();
    }

    //Called Automatically on The Client When This Client Gains Ownership of This NetworkObject
    public override void OnGainedOwnership()
    {
        base.OnGainedOwnership();
        Debug.Log($"OnGainedOwnership called for card {gameObject.name} on client {NetworkManager.Singleton.LocalClientId}");
        FlipFaceUp();
    }

    //Called Automatically on The Client When This Client Loses Ownership
    public override void OnLostOwnership()
    {
        base.OnLostOwnership();
        FlipFaceDown();
    }

    public void FlipFaceUp()
    {
        isFaceUp = true;
        StartFlip(true);
    }

    public void FlipFaceDown()
    {
        isFaceUp = false;
        StartFlip(false);
    }

    public void UpdateCardRotationInstant()
    {
        float zRot = ownerZRotation.Value;

        if (isFaceUp)
        {
            transform.rotation = Quaternion.Euler(-90f, 0f, zRot);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, zRot);
        }
    }

    private void StartFlip(bool faceUp)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        float zRot = ownerZRotation.Value;

        Debug.Log($"StartFlip called: faceUp={faceUp}, zRot={zRot}, IsOwner={IsOwner}");

        Quaternion startRot = transform.rotation;

        Quaternion targetRot;

        if (faceUp && IsOwner)
        {
            //Owner Sees Card Face up With Their Rotation
            targetRot = Quaternion.Euler(-90f, 0f, zRot);
        }
        else
        {
            //Non-Owner/ Face Down State
            targetRot = Quaternion.Euler(90f, 0f, zRot);
        }

        flipCoroutine = StartCoroutine(SmoothFlip(startRot, targetRot, flipDuration));
    }

    private IEnumerator SmoothFlip(Quaternion fromRot, Quaternion toRot, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(fromRot, toRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = toRot;
    }
}