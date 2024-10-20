using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    //Called From GameManager
    public void PlayCard()
    {
        //Damage opponent 
        if(GameManager.Instance.playerTurn == true)
        {
            //takes 1 finger away 
            GameManager.Instance.UpdateHealth(1);
        }
        if(GameManager.Instance.aiTurn == true)
        {
            //takes 1 finger away 
            GameManager.Instance.UpdateHealth(2);
        }
    }
}
