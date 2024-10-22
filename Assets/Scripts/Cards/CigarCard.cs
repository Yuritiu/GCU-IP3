using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigarCard : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Clone Players Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 2));

    }
    public void PlayCardForAI()
    {
        //Clone AI's Second Card
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 2));
    }
}
