using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardVisual : NetworkBehaviour
{
    public float passedZValue = 0f;

    public float flipDuration = 0.2f;
    public Coroutine flipCoroutine;
    public bool readyToFlip = false;
    bool calledFlip = false;
    bool isMine;

    public override void OnGainedOwnership()
    {
        TryFlipBasedOnOwnership();
    }

    public void TryFlipBasedOnOwnership()
    {
        if (!IsSpawned) return;

        isMine = NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId;

        if(!isMine) return;

        StartFlip(isMine);
    }

    public void StartFlip(bool faceUp)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        //Debug.Log("RUN FOR CLIENT ONLY");

        //Derive zRot Based on Player Seating Location

        //Debug.Log("Z Rotation " + passedZValue);
        Quaternion startRot = transform.rotation;
        //Face up For Local Player
        Quaternion targetRot = Quaternion.Euler(-90, transform.rotation.y, passedZValue);

        StartCoroutine(SmoothFlip(startRot, targetRot, flipDuration));
    }

    //public bool TryGetZRotation(out float zRot)
    //{
    //    zRot = passedZValue;
    //    Debug.Log("Z Rotation " + zRot);
    //    //Find InitialCardSpawner in Scene
    //    //var spawner = FindObjectOfType<InitialCardSpawner>();
    //    //if (spawner != null && spawner.playerZRotations.TryGetValue(NetworkObject.OwnerClientId, out float value))
    //    //{
    //    //    zRot = value;
    //    //    Debug.Log("Z Rotation " + zRot);
    //    //    return true;
    //    //}

    //    return false;
    //}

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