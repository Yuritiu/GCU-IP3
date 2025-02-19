using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingChamber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform chamber;

    [Header("Editable Variables")]
    [SerializeField] private float spinSpeed = 0.5f;
    [SerializeField] private float pauseBetweenLoads = 0.25f;
    [SerializeField] private float longSpinDuration = 1f;
    [SerializeField] private float longSpinSpeed = 700f;

    [Header("Tip Display")]
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private string[] tips;
    [SerializeField] private float tipWaveSpeed = 2f;
    [SerializeField] private float tipWaveAmplitude = 10f;

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

        if (tips.Length > 0)
        {
            tipText.gameObject.SetActive(true);
            tipText.text = "Tip: " + tips[Random.Range(0, tips.Length)];
            StartCoroutine(AnimateTip());
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

    private IEnumerator AnimateTip()
    {
        tipText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tipText.textInfo;
        Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        float time = 0f;

        while (true)
        {
            tipText.ForceMeshUpdate();
            textInfo = tipText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                //Create a wave effect on the Y axis Only
                float yOffset = Mathf.Sin(time + i * 0.3f) * tipWaveAmplitude;

                vertices[vertexIndex + 0] = originalVertices[materialIndex][vertexIndex + 0] + new Vector3(0, yOffset, 0);
                vertices[vertexIndex + 1] = originalVertices[materialIndex][vertexIndex + 1] + new Vector3(0, yOffset, 0);
                vertices[vertexIndex + 2] = originalVertices[materialIndex][vertexIndex + 2] + new Vector3(0, yOffset, 0);
                vertices[vertexIndex + 3] = originalVertices[materialIndex][vertexIndex + 3] + new Vector3(0, yOffset, 0);
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                tipText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            time += Time.deltaTime * tipWaveSpeed;
            yield return null;
        }
    }


}
