using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Skip Turn
        GameManager.Instance.aiSkipNextTurn = true;
    }
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away 
        GameManager.Instance.playerSkipTurn = true;
    }
}
