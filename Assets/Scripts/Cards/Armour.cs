using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : MonoBehaviour
{
    //Called From GameManager
    public void PlayCardForPlayer()
    {
        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        //if (rand == 0)
        //{
        //    //slows bloodloss
        //    BloodlossSystem.Instance.bloodlossTime -= BloodlossSystem.Instance.shieldBloodlossReduce;
        //    GameManager.Instance.armourBackfire.gameObject.SetActive(true);
        //    GameManager.Instance.playerArmour++;
        //}
        //else
        //{
            //Damage opponent 
            //takes 1 finger away 
            GameManager.Instance.playerArmour++;
        //}

    } 
           
    public void PlayCardForAI()
    {
        //Damage opponent 
        //takes 1 finger away 
        GameManager.Instance.aiArmour++;
    }
}
