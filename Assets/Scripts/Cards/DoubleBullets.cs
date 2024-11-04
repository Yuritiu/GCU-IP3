using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBullets : MonoBehaviour
{
    public void PlayCardForPlayer()
    {
        GameManager.Instance.addBullet();
    }

    public void PlayCardForAI()
    {
        GameManager.Instance.addBullet();
    }
}
