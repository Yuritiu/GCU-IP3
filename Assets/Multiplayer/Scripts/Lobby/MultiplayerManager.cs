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
    [SerializeField] private TMP_InputField joinCodeInputField;
    [Header("UI Button References")]
    [SerializeField] Button copyButton;
    [SerializeField] Button startGameButton;
    [SerializeField] Button leaveGameButton;

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

        leaveGameButton.onClick.AddListener(LeaveLobby);
    }

    #region UI Button Methods

    public async void OnCreateGameButtonPressed()
    {
        LobbyState.InLobby = true;

        loadingScreen.SetActive(true);

        await InitializeServicesAsync();
        await SignInAnonymouslyAsync();

        string playerName = GenerateRandomPlayerName();

        try
        {
            //Create Relay Allocation For 2 Players
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);
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
        LobbyState.InLobby = true;

        loadingScreen.SetActive(true);

        await InitializeServicesAsync();
        await SignInAnonymouslyAsync();

        string playerName = GenerateRandomPlayerName();

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

            Debug.Log("[MULTIPLAYER MANAGER] Client Joined Lobby With Code: " + codeFromUI);
        }
        catch (LobbyServiceException lse)
        {
            Debug.LogError($"[MULTIPLAYER MANAGER] [LOBBY ERROR] {lse.Message} | Reason: {lse.Reason} | Code: {lse.ErrorCode}");

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            LobbyState.InLobby = false;
        }
        catch (RelayServiceException rse)
        {
            Debug.LogError($"[MULTIPLAYER MANAGER] [CLIENT] RelayServiceException: {rse.Message}");

            connectRulebookUI.SetActive(true);
            lobbyRulebookUI.SetActive(false);
            loadingScreen.SetActive(false);

            LobbyState.InLobby = false;
        }
        catch (Exception e)
        {
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
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClientsList != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
            playersWaitingText.text = $"Players Waiting: {playerCount} / {maxPlayers}";

            //Enable Start Game Button When Minimum Players Joined
            startGameButton.interactable = NetworkManager.Singleton.IsHost && playerCount >= minPlayersToStart;
        }
        else
        {
            playersWaitingText.text = "Waiting For Host To Start...";
            startGameButton.interactable = false;
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

    public async void LeaveLobby()
    {
        loadingScreen.SetActive(true);

        try
        {
            if (currentLobby != null)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    //Host Deletes The Lobby
                    Debug.Log("[MULTIPLAYER MANAGER] [SERVER] Host/ Server Shutting Down...");
                    NetworkManager.Singleton.Shutdown();
                }
                else if(NetworkManager.Singleton.IsClient)
                {
                    //Client Leaves
                    Debug.Log("[MULTIPLAYER MANAGER] [SERVER] Client Disconnecting...");
                    NetworkManager.Singleton.Shutdown();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[MULTIPLAYER MANAGER] Error While Leaving The Lobby: " + e.Message);
        }

        //Shutdown The Network Manager
        if (networkManager.IsHost || networkManager.IsServer)
            networkManager.Shutdown();
        else if (networkManager.IsClient)
            networkManager.Shutdown();

        currentLobby = null;

        //Reset UI
        lobbyRulebookUI.SetActive(false);
        connectRulebookUI.SetActive(true);
        joinCodeText.text = "";
        playersWaitingText.text = "";
        playerNamesDict.Clear();
        networkPlayerName.playerListText.text = "";

        //Reset Camera
        MenuCameraController.Instance.ReturnToOrbit();

        loadingScreen.SetActive(false);

        Debug.Log("[MULTIPLAYER MANAGER] [NETWORK] Left Lobby And Shut Down Networking.");

        LobbyState.InLobby = false;
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

    #region Player Name Generator

    string GenerateRandomPlayerName()
    {
        string[] dinosaurs = { "Ankylosaurus", "T-Rex", "Spinosaurus", "Stegosaurus", "Carnotaurus", "Parasaurolophus" };
        string[] names = { "Jordan", "Charlie", "Benjamin", "Rebecca", "Agnes", "Mick", "Jack", "James", "Josh", "Kyle" };
        var random = new System.Random();

        string adjective = dinosaurs[random.Next(dinosaurs.Length)];
        string animal = names[random.Next(names.Length)];

        return $"{animal} The {adjective}";
    }

    #endregion
}