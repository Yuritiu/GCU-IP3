using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        //Damage opponent 
        //takes 1 finger away 

        GameManager.Instance.playerArmour++;
    } 
           
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away 
        GameManager.Instance.aiArmour++;
    }
}
