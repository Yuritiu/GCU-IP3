using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;

public class DataGathering : MonoBehaviour
{
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdrKEyYws7DYWxErsqC6T_1GpJ_-JhAjlC6UebcuuBecETt1Q/formResponse";

    //Basic
    [HideInInspector] public bool won; //True = Player, False = Opponent
    [HideInInspector] public bool bloodLost = false;  //True = Yes, False = No
    [HideInInspector] public int fingerslostPlayer;
    [HideInInspector] public int fingerslostOpponent;
    [HideInInspector] public bool gunLoss; //True = Yes, False =No

    //All Card Used + Backfire
    [HideInInspector] public int knifeUsed;
    [HideInInspector] public int kinfebackfire;
    [HideInInspector] public int oneChamberUsed;
    [HideInInspector] public int oneChamberBackfire;
    [HideInInspector] public int gunUsed;
    [HideInInspector] public int gunBackfire;
    [HideInInspector] public int cigarUsed;
    [HideInInspector] public int cigarBackfire;
    [HideInInspector] public int bottleUsed;
    [HideInInspector] public int bottleBackfire;
    [HideInInspector] public int armorUsed;
    [HideInInspector] public int armorBackfire;
    [HideInInspector] public int emptyUsed;
    [HideInInspector] public int emptyBackfire;

    public void Start()
    {
        //Send();
    }

    public void GamEnded(bool whoWon)
    {
        won = whoWon;
       StartCoroutine(Post());
    }

    IEnumerator Post()
    {
        if (!Application.isEditor)
        {
            WWWForm form = new WWWForm();

            //Player or Opponent Won
            if (won == true)
            {
                form.AddField("entry.1739160518", "Player");
            }
            else if (won == false)
            {
                form.AddField("entry.1739160518", "Opponent");
            }

            //Was it bloodloss?
            if (bloodLost == true)
            {
                form.AddField("entry.1258567324", "Yes");
            }
            else if (bloodLost == false)
            {
                form.AddField("entry.1258567324", "No");
            }

            //Was it bloodloss?
            if (gunLoss == true)
            {
                form.AddField("entry.93545155", "Yes");
            }
            else if (gunLoss == false)
            {
                form.AddField("entry.93545155", "No");
            }

            //FingersLostPlayer
            form.AddField("entry.1283918594", fingerslostPlayer);

            //FingersLostOpponent
            form.AddField("entry.2044717277", fingerslostOpponent);



            UnityWebRequest www = UnityWebRequest.Post(URL, form);

            yield return www.SendWebRequest();
        }
    }
}
