using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.Timeline;

public class DataGathering : MonoBehaviour
{
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdrKEyYws7DYWxErsqC6T_1GpJ_-JhAjlC6UebcuuBecETt1Q/formResponse";
    [SerializeField] bool InTesting = false;
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
    [HideInInspector] public int armorUsed;
    [HideInInspector] public int armorBackfire;
    [HideInInspector] public int emptyUsed;
    [HideInInspector] public int emptyBackfire;

    //AI
    [HideInInspector] public int knifeUsedAI;
    [HideInInspector] public int kinfebackfireAI;
    [HideInInspector] public int oneChamberUsedAI;
    [HideInInspector] public int oneChamberBackfireAI;
    [HideInInspector] public int gunUsedAI;
    [HideInInspector] public int gunBackfireAI;
    [HideInInspector] public int cigarUsedAI;
    [HideInInspector] public int cigarBackfireAI;
    [HideInInspector] public int bottleUsedAI;
    [HideInInspector] public int armorUsedAI;
    // [HideInInspector] public int armorBackfireAI; //armorBackfire commented out for when added bloodloss
    [HideInInspector] public int emptyUsedAI;
    [HideInInspector] public int emptyBackfireAI;
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
        if (!Application.isEditor || InTesting)
        {
            WWWForm form = new WWWForm();

            BasicMatchData(form);

            PlayerCardData(form);

            AICardData(form);
            UnityWebRequest www = UnityWebRequest.Post(URL, form);

            yield return www.SendWebRequest();
       }
    }

    void BasicMatchData(WWWForm form)
    {
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
    }

    void PlayerCardData(WWWForm form)
    {
        //Player Card
        //Knife
        form.AddField("entry.702118825", knifeUsed);

        //KnifeBackfire
        form.AddField("entry.1076525665", kinfebackfire);

        //OneCHamber
        form.AddField("entry.1335241368", oneChamberUsed);

        //OneCHamberBack
        form.AddField("entry.1511864571", oneChamberBackfire);

        //gun
        form.AddField("entry.2080031071", gunUsed);

        //gunback
        form.AddField("entry.2014009783", gunBackfire);

        //Cigar
        form.AddField("entry.473309497", cigarUsed);

        //CigarBack
        form.AddField("entry.417711706", cigarBackfire);

        //Bottle
        form.AddField("entry.1993230955", bottleUsed);

        //Armor
        form.AddField("entry.1096992637", armorUsed);

        //ArmorBack
        form.AddField("entry.1577371329", armorBackfire);

        //EmptyUsed
        form.AddField("entry.1095532774", emptyUsed);

        //EmptyUsedBack
        form.AddField("entry.268483346", emptyBackfire);
    }

    void AICardData(WWWForm form)
    {
        //AI Card
        //Knife
        form.AddField("entry.2036979771", knifeUsedAI);

        //KnifeBackfire
        form.AddField("entry.1609314367", kinfebackfireAI);

        //OneCHamber
        form.AddField("entry.1981386679", oneChamberUsedAI);

        //OneCHamberBack
        form.AddField("entry.326105874", oneChamberBackfireAI);

        //gun
        form.AddField("entry.1546682645", gunUsedAI);

        //gunback
        form.AddField("entry.1339083574", gunBackfireAI);

        //Cigar
        form.AddField("entry.595216909", cigarUsedAI);

        //CigarBack
        form.AddField("entry.77491581", cigarBackfireAI);

        //Bottle
        form.AddField("entry.900074417", bottleUsedAI);

        //Armor
        form.AddField("entry.408903274", armorUsedAI);

        //ArmorBack
        //form.AddField(" ", armorBackfire); //Will be enabled if/when the AI has bloodloss

        //EmptyUsed
        form.AddField("entry.161898756", emptyUsedAI);

        //EmptyUsedBack
        form.AddField("entry.1650103874", emptyBackfireAI);
    }
}
