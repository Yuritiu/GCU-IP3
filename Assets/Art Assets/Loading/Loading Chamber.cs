using System.Collections;
using UnityEngine;

public class LoadingChamber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform chamber;

    [Header("Editable Variables")]
    [SerializeField] private float spinSpeed = 0.5f;
    [SerializeField] private float pauseBetweenLoads = 0.25f;
    [SerializeField] private float longSpinDuration = 1f;
    [SerializeField] private float longSpinSpeed = 700f;

    private int currentBulletIndex = 0;
    private Transform[] bullets;

    void Start()
    {
        bullets = new Transform[chamber.childCount];

        for (int i = 0; i < chamber.childCount; i++)
        {
            bullets[i] = chamber.GetChild(i);
            bullets[i].gameObject.SetActive(false);
        }

        StartCoroutine(LoadingLoop());
    }

    private IEnumerator LoadingLoop()
    {
        //Infinite loading loop
        while (true) 
        {
            yield return StartCoroutine(LoadChamberSequence());
            yield return StartCoroutine(LongSpinAndDeload());
        }
    }

    private IEnumerator LoadChamberSequence()
    {
        currentBulletIndex = 0;

        while (currentBulletIndex < bullets.Length)
        {
            //Spin then load bullet
            yield return StartCoroutine(SpinChamber(spinSpeed)); 
            bullets[currentBulletIndex].gameObject.SetActive(true);
            currentBulletIndex++;
            yield return new WaitForSeconds(pauseBetweenLoads);
        }
    }

    private IEnumerator LongSpinAndDeload()
    {
        //Removes bullets based on duration of spin 1 by 1 to create the "deload" effect
        float elapsedTime = 0f;
        float bulletRemoveInterval = longSpinDuration / bullets.Length;

        for (int i = bullets.Length - 1; i >= 0; i--)
        {
            float removeTime = 0f;

            while (removeTime < bulletRemoveInterval)
            {
                float rotationAmount = longSpinSpeed * Time.deltaTime;
                chamber.Rotate(0, 0, -rotationAmount);
                removeTime += Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            bullets[i].gameObject.SetActive(false);
        }

        while (elapsedTime < longSpinDuration)
        {
            float rotationAmount = longSpinSpeed * Time.deltaTime;
            chamber.Rotate(0, 0, -rotationAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        chamber.rotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator SpinChamber(float speed)
    {
        //60 degrees rotation per bullet (angle distance I put between in illustrator)
        float elapsedTime = 0f;
        float targetRotation = chamber.rotation.eulerAngles.z - 60f;

        //Anticipation for the chamber like your cranking it yourself
        float anticipationRotation = chamber.rotation.eulerAngles.z + 10f;

        //Anticipation spin
        while (elapsedTime < speed / 2)
        {
            float zRotation = Mathf.LerpAngle(chamber.rotation.eulerAngles.z, anticipationRotation, elapsedTime / (speed / 2));
            chamber.rotation = Quaternion.Euler(0, 0, zRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Normal spin
        elapsedTime = 0f;
        while (elapsedTime < speed)
        {
            float zRotation = Mathf.LerpAngle(chamber.rotation.eulerAngles.z, targetRotation, elapsedTime / speed);
            chamber.rotation = Quaternion.Euler(0, 0, zRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        chamber.rotation = Quaternion.Euler(0, 0, targetRotation);
    }
}
