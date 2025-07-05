using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardVisual : NetworkBehaviour
{
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

    public bool TryGetZRotation(out float zRot)
    {
        zRot = 0f;

        //Find InitialCardSpawner in Scene
        var spawner = FindObjectOfType<InitialCardSpawner>();
        //TODO: ------------------------ CHANGE THIS FROM OWNER CLIENT ID BECAUSE SERVER IS OWNER CLIENT ID NEEDS TO BE LOCAL -------------------------
        if (spawner != null && spawner.playerZRotations.TryGetValue(NetworkObject.OwnerClientId, out float value))
        {
            zRot = value;
            return true;
        }

        return false;
    }

    public void StartFlip(bool faceUp)
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        Debug.Log("RUN FOR CLIENT ONLY");
        float zRot = 0f;

        //Derive zRot Based on Player Seating Location
        if (TryGetZRotation(out float playerZ))
            zRot = playerZ;

        Quaternion startRot = transform.rotation;
        //Face up For Local Player
        Quaternion targetRot = Quaternion.Euler(-90, transform.rotation.y, zRot);

        StartCoroutine(SmoothFlip(startRot, targetRot, flipDuration));
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