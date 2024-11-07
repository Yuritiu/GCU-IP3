using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] List<GameObject> fingers;


    public void RemoveFinger(int num, bool isAI)
    {
        if (isAI && GameManager.Instance.aiArmour > 0)
            return;

        if (!isAI && GameManager.Instance.playerArmour > 0)
            return;

        Destroy(fingers[num]);
        fingers.Remove(fingers[num]);
    }
}
