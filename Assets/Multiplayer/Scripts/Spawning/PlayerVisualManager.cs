using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisualManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerVisualModel;
    [SerializeField] private GameObject opponentModel;

    void Start()
    {
        InitializeVisuals();
    }

    /// <summary>
    /// Disables local player model and enables everyone elses
    /// </summary>
    public void InitializeVisuals()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("Player Model Shown for local player");
            //Only Show Player Model For Local Player
            if (playerVisualModel != null) playerVisualModel.SetActive(true);
            if (opponentModel != null) opponentModel.SetActive(false);
        }
        else
        {
            Debug.Log("Opponent Model Shown for other players");
            //Hide All Others Player Model & Show The Opponent Model
            if (playerVisualModel != null) playerVisualModel.SetActive(false);
            if (opponentModel != null) opponentModel.SetActive(true);
        }
    }

    /// <summary>
    /// Forces both player and opponent models to show -> useful for when we compare cards and the camera is panning around
    /// </summary>
    public void ShowAllVisuals()
    {
        if (playerVisualModel != null) playerVisualModel.SetActive(false);
        if (opponentModel != null) opponentModel.SetActive(true);
    }

    //-------------------------------- CALLING --------------------------------
    //IN SCRIPT WHEN WE CONTROL CAMERA PANNING CALL SOMETHING LIKE THIS TO CALL THE FUNCTIONS (i think)
    // Show all visuals for external camera
    //foreach (var handler in FindObjectsOfType<PlayerVisualHandler>())
    //{
    //    handler.ShowAllVisuals();
    //}

    //// Reinitialize all player visuals (like Start does)
    //foreach (var handler in FindObjectsOfType<PlayerVisualHandler>())
    //{
    //    handler.InitializeVisuals();
    //}
}