using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSmash : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Destroy The Bottle When Hits AI/ Player
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent"))
        {
            Destroy(gameObject);
        }
    }
}
