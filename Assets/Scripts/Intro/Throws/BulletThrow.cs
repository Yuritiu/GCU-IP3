using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThrow : MonoBehaviour
{
    public static BulletThrow Instance;

    [Header("Casings & Target Settings")]
    [SerializeField] public List<Transform> casings;
    [SerializeField] public List<Transform> targetPoints;
    [SerializeField] public List<GameObject> visibleCasings;
    [SerializeField] public Transform chamber;

    [Header("Ejection Settings")]
    float ejectionSpeed = 1.5f;
    float moveSpeed = 2f;
    float rotationSpeed = 2f;
    float delayBetweenEjections = 0.3f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (casings.Count != targetPoints.Count)
        {
            Debug.LogError("Casings and Target Points Lists Don't Have The Same No Of Assignments");
            return;
        }
    }

    public void StartEjection()
    {
        StartCoroutine(EjectCasings());
    }

    IEnumerator EjectCasings()
    {
        for (int i = 0; i < casings.Count; i++)
        {
            StartCoroutine(EjectCasing(casings[i], targetPoints[i]));
            yield return new WaitForSeconds(delayBetweenEjections);
        }
    }

    IEnumerator EjectCasing(Transform casing, Transform target)
    {
        Vector3 floatBackPos = casing.position + casing.right * 0.1f;

        //Float Out
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime * ejectionSpeed;
            casing.position = Vector3.Lerp(casing.position, floatBackPos, t);
            yield return null;
        }

        //Lerp To Pos & Rotation
        t = 0;
        Quaternion startRotation = casing.rotation;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            casing.position = Vector3.Lerp(floatBackPos, target.position, t);
            casing.rotation = Quaternion.Slerp(startRotation, target.rotation, t * rotationSpeed);
            yield return null;
        }

        StartCoroutine(RotateChamber());
    }

    IEnumerator RotateChamber()
    {
        if (chamber == null) yield break;

        yield return new WaitForSeconds(1.5f);

        foreach (Transform casing in casings)
        {
            casing.gameObject.SetActive(false);
        }

        foreach (GameObject newCasing in visibleCasings)
        {
            newCasing.SetActive(true);
        }

        float startZ = chamber.eulerAngles.z;
        float targetZ = startZ -18;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 2;
            float newZ = Mathf.Lerp(startZ, targetZ, t);
            chamber.rotation = Quaternion.Euler(chamber.eulerAngles.x, chamber.eulerAngles.y, newZ);
            yield return null;
        }

        chamber.rotation = Quaternion.Euler(chamber.eulerAngles.x, chamber.eulerAngles.y, targetZ);
    }
}
