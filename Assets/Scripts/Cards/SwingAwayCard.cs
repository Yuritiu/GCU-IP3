using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingAwayCard : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Skip AI Turn
        GameManager.Instance.aiSkippedTurns++;
    }
    public void PlayCardForAI()
    {
        //Skip Players Turn
        GameManager.Instance.playerSkippedTurns++;
    }
}
