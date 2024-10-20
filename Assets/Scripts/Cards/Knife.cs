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
        GameManager.Instance.UpdateHealth(1);

    }
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away 
        GameManager.Instance.UpdateHealth(2);
    }
}
