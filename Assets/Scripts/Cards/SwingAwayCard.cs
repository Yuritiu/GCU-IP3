using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    //Called From GameManager
    public void PlayCard()
    {
        //Skip Turn
        GameManager.Instance.aiSkipNextTurn = true;
    }
}
