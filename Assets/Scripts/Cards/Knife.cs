using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Damage opponent 
        //takes 1 finger away
        StartCoroutine(GameManager.Instance.WaitToCompareCards(1, 1));
    }
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away
        StartCoroutine(GameManager.Instance.WaitToCompareCards(2, 1));
    }
}
