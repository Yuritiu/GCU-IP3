using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Networking.Transport.Relay;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] UnityTransport unityTransport;
    [SerializeField] NetworkPlayerName networkPlayerName;

    [Header("UI References")]
    [SerializeField] public GameObject lobbyRulebookUI;
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public GameObject connectRulebookUI;
    [SerializeField] public TextMeshProUGUI playersWaitingText;
    [SerializeField] public TextMeshProUGUI joinCodeText;
    [SerializeField] TMP_InputField joinCodeInputField;
    [Header("UI Button References")]
    [SerializeField] Button copyButton;
    [SerializeField] Button startGameButton;
    [SerializeField] Button leaveGameButton;
    [Header("Specific UI Cases")]
    [SerializeField] GameObject startGameText;
    [SerializeField] GameObject leaveButtonOutline;
    [Header("Player Name UI References")]
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] PlayerNameInputManager nameInputManager;
    [Header("UI Error Text References")]
    [SerializeField] public TextMeshProUGUI joinGameErrorText;

    [Header("Network")]
    [SerializeField] private NetworkManager networkManager;
    //Dictionary Synced Across Network: clientId -> playerName
    [SerializeField] private NetworkVariable<FixedString128Bytes> syncedPlayerNames = new();

    //Local Dictionary Parsed From syncedPlayerNames NetworkVariable
    Dictionary<string, string> playerNamesDict = new Dictionary<string, string>();

    [Header("Lobby Variables")]
    Lobby currentLobby;
    float lobbyRefreshInterval = 2f;
    float lobbyRefreshTimer = 0f;
    //CHANGE TO 2
    const int minPlayersToStart = 1;
    const int maxPlayers = 4;

    void Start()
    {
        startGameButton.interactable = false;
        joinGameErrorText.text = "";

        leaveGameButton.onClick.AddListener(LeaveLobby);
    }

    #region Handle Client Disconnects When Host Leaves

    void OnEnable()
    {
        StartCoroutine(SubscribeWhenNetworkManagerReady());
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    IEnumerator SubscribeWhenNetworkManagerReady()
    {
        while (NetworkManager.Singleton == null)
        {
            //Wait For NetworkManager.Singleton to Exist
            yield return null;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        Debug.Log("[CLIENT] Subscribed to OnClientDisconnectCallback");
    }

    void HandleClientDisconnected(ulong clientId)
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;

        if (clientId == myClientId)
        {
            Debug.Log($"[CLIENT] I Got Disconnected From The Server. ClientId: {clientId}");

            ResetUI();
            currentLobby = null;
            LobbyState.InLobby = false;
        }
        else
        {
            Debug.Log($"[CLIENT] Another Client Disconnected From The Server (ClientId: {clientId})");
        }
    }

    #endregion

    #region UI Button Methods

    public async void OnCreateGameButtonPressed()
    {
        if (!nameInputManager.ValidateAndSaveInput()) return;

        LobbyState.InLobby = true;

        loadingScreen.SetActive(true);

        await InitializeServicesAsync();
        await SignInAnonymouslyAsync();

        //------------ Handle Player Name ------------
        //Check If The Player Is a Developer
        string playerName = PlayerNameInputManager.RawPlayerName;
        var devManager = FindObjectOfType<DeveloperIdentityManager>();
        var idManager = FindObjectOfType<PlayerIdentityManager>();
        if (devManager != null && idManager != null && devManager.IsDeveloper(idManager.PlayerID))
        {
            playerName = "[DEV] " + playerName;
        }
        //------------ End Of Handle Player Name ------------

        try
        {
            //Create Relay Allocation For 4 Players
            var allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            //Enable NetworkManager
            if (!networkManager.gameObject.activeInHierarchy)
            {
                networkManager.gameObject.SetActive(true);
            }

            //Set Relay Data On Transport
            var relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);

            networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //Create Lobby And Add Player Name
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                    }
                },
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode.Trim().ToUpper()) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("MyLobby", maxPlayers, options);
            //Display Join Code From Official LobbyCode Property For Consistency
            joinCodeText.text = currentLobby.LobbyCode.Trim().ToUpper();

            copyButton.onClick.RemoveAllListeners();
            copyButton.onClick.AddListener(() => CopyToClipboard(currentLobby.LobbyCode));

            //Start Host
            if (!networkManager.IsListening)
            {
                networkManager.StartHost();
            }

            UpdatePlayerNamesFromLobby();

            connectRulebookUI.SetActive(false);
            lobbyRulebookUI.SetActive(true);
            loadingScreen.SetActive(false);

            Debug.Log("[MULTIPLAYER MANAGER] Host Started With Join Code: " + joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError("[MULTIPLAYER MANAGER] Create Game Failed: " + e.Message);

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            //Disable NetworkManager
            if (!networkManager.gameObject.activeInHierarchy)
            {
                networkManager.gameObject.SetActive(false);
            }

            LobbyState.InLobby = false;
        }
    }

    public async void OnJoinGameButtonPressed()
    {
        if (!nameInputManager.ValidateAndSaveInput()) return;

        LobbyState.InLobby = true;
        loadingScreen.SetActive(true);
        joinGameErrorText.text = "";

        await InitializeServicesAsync();
        await SignInAnonymouslyAsync();

        //------------ Handle Player Name ------------
        //Check If The Player Is a Developer
        string playerName = PlayerNameInputManager.RawPlayerName;
        var devManager = FindObjectOfType<DeveloperIdentityManager>();
        var idManager = FindObjectOfType<PlayerIdentityManager>();
        if (devManager != null && idManager != null && devManager.IsDeveloper(idManager.PlayerID))
        {
            playerName = "[DEV] " + playerName;
        }
        //------------ End Of Handle Player Name ------------

        string codeFromUI = joinCodeInputField.text;

        try
        {
            Debug.Log("[MULTIPLAYER MANAGER] [CLIENT] Trying to join lobby...");
            //Join The Lobby Using The Code And Add Player Data
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(codeFromUI, new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                {
                    { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                }
                }
            });
            Debug.Log($"[MULTIPLAYER MANAGER] [CLIENT] Joined lobby successfully: {currentLobby.Id}");

            if (!currentLobby.Data.TryGetValue("joinCode", out var joinCodeData))
            {
                joinGameErrorText.text = "Join code is missing from the lobby.";
                Debug.LogError("[MULTIPLAYER MANAGER] Join Code Not Found In Lobby Data!");
                throw new Exception("Join code missing from lobby.");
            }

            string relayJoinCode = joinCodeData.Value.Trim().ToUpper();
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            var relayServerData = new RelayServerData(joinAllocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);

            if (!networkManager.gameObject.activeInHierarchy)
            {
                networkManager.gameObject.SetActive(true);
            }

            //Start Client
            if (!networkManager.IsListening)
            {
                networkManager.StartClient();
            }

            UpdatePlayerNamesFromLobby();

            connectRulebookUI.SetActive(false);
            lobbyRulebookUI.SetActive(true);
            loadingScreen.SetActive(false);

            //Display Join Code For Client Too
            joinCodeText.text = currentLobby.LobbyCode.Trim().ToUpper();
            
            //Force UI update after join
            Invoke(nameof(UpdatePlayerListUI), 1f);

            joinGameErrorText.text = "";
            Debug.Log("[MULTIPLAYER MANAGER] Client Joined Lobby With Code: " + codeFromUI);
        }
        catch (LobbyServiceException lse)
        {
            joinGameErrorText.text = $"Failed to join lobby: {lse.Reason} (Code: {lse.ErrorCode})";
            Debug.LogError($"[MULTIPLAYER MANAGER] [LOBBY ERROR] {lse.Message} | Reason: {lse.Reason} | Code: {lse.ErrorCode}");

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            LobbyState.InLobby = false;
        }
        catch (RelayServiceException rse)
        {
            joinGameErrorText.text = $"Relay error: {rse.Message}";
            Debug.LogError($"[MULTIPLAYER MANAGER] [CLIENT] RelayServiceException: {rse.Message}");

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            LobbyState.InLobby = false;
        }
        catch (Exception e)
        {
            joinGameErrorText.text = $"Could not join: {e.Message}";
            Debug.LogError($"[MULTIPLAYER MANAGER] Failed To Join Lobby With Code '{codeFromUI}': {e.Message}\n{e.StackTrace}");

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            LobbyState.InLobby = false;
        }
    }

    public void OnStartGameButtonPressed()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        if (!NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started Game");
        }

        //Load The Multiplayer Game Scene For All Clients
        NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer Game Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #endregion

    void Update()
    {
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsHost)
        {
            //HOST UI Logic
            if (NetworkManager.Singleton.ConnectedClientsList != null)
            {
                int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
                playersWaitingText.text = $"Players Waiting: {playerCount} / {maxPlayers}";

                startGameButton.interactable = playerCount >= minPlayersToStart;
                startGameButton.enabled = playerCount >= minPlayersToStart;
                startGameText.SetActive(playerCount >= minPlayersToStart);
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            //CLIENT UI Logic
            playersWaitingText.text = "Waiting for host to start...";
            startGameText.SetActive(false);
            startGameButton.interactable = false;
            startGameButton.enabled = false;
        }

        if (currentLobby != null)
        {
            lobbyRefreshTimer += Time.deltaTime;
            if (lobbyRefreshTimer >= lobbyRefreshInterval)
            {
                lobbyRefreshTimer = 0f;
                RefreshLobbyDataAsync();
            }
        }
    }

    void CopyToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    #region Networking Lobby Data

    async void RefreshLobbyDataAsync()
    {
        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            UpdatePlayerNamesFromLobby();

            if (!NetworkManager.Singleton.IsHost)
            {
                //Display Join Code For Client Too
                joinCodeText.text = currentLobby.LobbyCode.Trim().ToUpper();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[MULTIPLAYER MANAGER] Failed to Refresh Lobby Data: " + e.Message);
        }
    }

    #region Update Player Name
    void UpdatePlayerNamesFromLobby()
    {
        playerNamesDict.Clear();

        foreach (var player in currentLobby.Players)
        {
            if (player.Data.TryGetValue("name", out PlayerDataObject nameData))
            {
                playerNamesDict[player.Id] = nameData.Value;
            }
        }

        UpdatePlayerListUI();
    }

    void UpdatePlayerListUI()
    {
        var sb = new StringBuilder();
        foreach (var name in playerNamesDict.Values)
            sb.AppendLine(name);

        networkPlayerName.playerListText.text = sb.ToString();
    }
    #endregion

    public async void LeaveLobby()
    {
        loadingScreen.SetActive(true);

        try
        {
            if (currentLobby != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    //Host Deletes The Lobby on The Backend
                    await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                    NetworkManager.Singleton.Shutdown();

                    Debug.Log("[MULTIPLAYER MANAGER] Host deleted the lobby.");
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    //Client Removes Themselves From The Lobby on The Backend
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                    NetworkManager.Singleton.Shutdown();

                    Debug.Log("[MULTIPLAYER MANAGER] Client removed from lobby.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[MULTIPLAYER MANAGER] Error While Leaving The Lobby: " + e.Message);
        }

        //currentLobby = null;

        //ResetUI();

        //LobbyState.InLobby = false;

        Debug.Log("[MULTIPLAYER MANAGER] [NETWORK] Left Lobby And Shut Down Networking.");
    }

    #endregion

    #region UI Functions

    void ResetUI()
    {
        //Reset UI
        lobbyRulebookUI.SetActive(false);
        connectRulebookUI.SetActive(true);
        leaveButtonOutline.SetActive(false);


        joinCodeText.text = "";
        playersWaitingText.text = "";
        playerNamesDict.Clear();
        networkPlayerName.playerListText.text = "";

        //Reset Camera
        MenuCameraController.Instance.ReturnToOrbit();

        loadingScreen.SetActive(false);
    }

    #endregion

    #region Unity Services

    async System.Threading.Tasks.Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[MULTIPLAYER MANAGER] Unity Services Already Initialized or Failed to Initialize: {e.Message}");
        }
    }

    async System.Threading.Tasks.Task SignInAnonymouslyAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in Anonymously as {AuthenticationService.Instance.PlayerId}");
        }
    }

    #endregion
}