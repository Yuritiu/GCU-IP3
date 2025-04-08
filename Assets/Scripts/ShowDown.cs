using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class ShowDown : MonoBehaviour
{
    public bool showdown;
    public static ShowDown instance;
    public GameObject PlayerGun;
    public GameObject AiGun;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        showdown = false;
    }
    public void ActivateShowdown()
    {
        showdown = true;
        playShowDown();
    }

    private void Update()
    {
        if (CardDeck.Instance.deck.Count == 0 && AICardDrawSystem.Instance.cardsInHand.Length == 0 && CardDrawSystem.Instance.cardsInHand.Length == 0)
        {
            ActivateShowdown();
        }    
    }


    public IEnumerator playShowDown()
    {
        while (showdown)
        {
            if (PlayerGun.activeInHierarchy == false && AiGun.activeInHierarchy == false)
            {
                GameManager.Instance.PlayerRoulette();

            }
            
            if (PlayerGun.activeInHierarchy == true && AiGun.activeInHierarchy == false )
            {
                GameManager.Instance.AiRoulette();
            }

            if (PlayerGun.activeInHierarchy == false && AiGun.activeInHierarchy == true )
            {
                GameManager.Instance.PlayerRoulette();
            }
            
            yield return new WaitForSeconds(2);
            GameManager.Instance.AiRoulette();
                
            
            yield return null;
        } 
            
        yield return null;
        
    }
}
