using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBullets : MonoBehaviour
{
    public void PlayCardForPlayer()
    {
        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //loads blank (does nothing)
            GameManager.Instance.twoInChamberBackfire.gameObject.SetActive(true);
            return;
        }
        GameManager.Instance.addBullet();
    }

    public void PlayCardForAI()
    {
        int rand = Random.Range(0, 5);
        //\/Debuging\/
        //rand = 0;
        //print(rand);
        if (rand == 0)
        {
            //loads blank (does nothing)
            return;
        }
        GameManager.Instance.addBullet();
    }
}
