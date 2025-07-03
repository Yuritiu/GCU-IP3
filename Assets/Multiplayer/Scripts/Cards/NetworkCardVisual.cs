using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardVisual : NetworkBehaviour
{
    public float flipDuration = 0.2f;
    private Coroutine flipCoroutine;

    public override void OnGainedOwnership()
    {
        TryFlipBasedOnOwnership();
    }

    public void TryFlipBasedOnOwnership()
    {
        if (!IsSpawned) return;

        bool isMine = NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId;
        StartFlip(isMine);
    }

    private void StartFlip(bool faceUp)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        float zRot = 0f;

        //Derive zRot Based on Player Seating Location
        if (TryGetZRotation(out float playerZ))
            zRot = playerZ;

        Quaternion startRot = transform.rotation;
        //Face up For Local Player
        Quaternion targetRot = Quaternion.Euler(-90f, 0f, zRot); 

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

    private bool TryGetZRotation(out float zRot)
    {
        zRot = 0f;

        //Find InitialCardSpawner in Scene
        var spawner = FindObjectOfType<InitialCardSpawner>();
        if (spawner != null && spawner.playerZRotations.TryGetValue(NetworkObject.OwnerClientId, out float value))
        {
            zRot = value;
            return true;
        }

        return false;
    }
}