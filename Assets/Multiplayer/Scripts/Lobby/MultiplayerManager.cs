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

public class MultiplayerManager : MonoBehaviour
{
    //---------------------------------------------------------------------------------
    //TODO: REFRESH PLAYER LIST TEXT ON HOST MACHINE WHEN PLAYER JOINS,
    // FOR PLAYER THAT JOINED, FILL THE JOIN TEXT WITH THE CODE,
    // ADD LEAVING & AUTOMATIC DISCONNECTING IN UI
    //---------------------------------------------------------------------------------
    [SerializeField] UnityTransport unityTransport;
    [SerializeField] NetworkPlayerName networkPlayerName;

    [Header("UI References")]
    [SerializeField] public GameObject lobbyScreen;
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public GameObject preMultiplayerScreen;
    [SerializeField] public TextMeshProUGUI playersWaitingText;
    [SerializeField] public TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] Button copyButton;
    [SerializeField] Button startGameButton;

    [Header("Network")]
    [SerializeField] private NetworkManager networkManager;
    //Dictionary Synced Across Network: clientId -> playerName
    [SerializeField] private NetworkVariable<FixedString128Bytes> syncedPlayerNames = new();

    //Local Dictionary Parsed From syncedPlayerNames NetworkVariable
    Dictionary<string, string> playerNamesDict = new Dictionary<string, string>();

    [Header("Lobby Variables")]
    Lobby currentLobby;
    bool isHost;
    float lobbyRefreshInterval = 2f;
    float lobbyRefreshTimer = 0f;
    const int minPlayersToStart = 2;
    const int maxPlayers = 4;

    void Start()
    {
        startGameButton.interactable = false;
    }

    #region UI Button Methods

    public async void OnCreateGameButtonPressed()
    {
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

            isHost = true;

            UpdatePlayerNamesFromLobby();

            preMultiplayerScreen.SetActive(false);
            lobbyScreen.SetActive(true);
            loadingScreen.SetActive(false);

            Debug.Log("Host Started With Join Code: " + joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError("Create Game Failed: " + e.Message);

            preMultiplayerScreen.SetActive(true);
            lobbyScreen.SetActive(false);
            loadingScreen.SetActive(false);

            //Disable NetworkManager
            if (!networkManager.gameObject.activeInHierarchy)
            {
                networkManager.gameObject.SetActive(false);
            }
        }
    }

    public async void OnJoinGameButtonPressed()
    {
        loadingScreen.SetActive(true);

        await InitializeServicesAsync();
        await SignInAnonymouslyAsync();

        string playerName = GenerateRandomPlayerName();

        isHost = false;

        string codeFromUI = joinCodeInputField.text;

        try
        {
            Debug.Log("[CLIENT] Trying to join lobby...");
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
            Debug.Log($"[CLIENT] Joined lobby successfully: {currentLobby.Id}");

            if (!currentLobby.Data.TryGetValue("joinCode", out var joinCodeData))
            {
                Debug.LogError("Join code not found in lobby data.");
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

            isHost = false;

            UpdatePlayerNamesFromLobby();

            preMultiplayerScreen.SetActive(false);
            lobbyScreen.SetActive(true);
            loadingScreen.SetActive(false);

            //Display Join Code For Client Too
            joinCodeText.text = currentLobby.LobbyCode.Trim().ToUpper();
            
            //Force UI update after join
            Invoke(nameof(UpdatePlayerListUI), 1f);

            Debug.Log("Client Joined Lobby With Code: " + codeFromUI);
        }
        catch (LobbyServiceException lse)
        {
            Debug.LogError($"[LOBBY ERROR] {lse.Message} | Reason: {lse.Reason} | Code: {lse.ErrorCode}");

            preMultiplayerScreen.SetActive(true);
            lobbyScreen.SetActive(false);
            loadingScreen.SetActive(false);
        }
        catch (RelayServiceException rse)
        {
            Debug.LogError($"[CLIENT] RelayServiceException: {rse.Message}");

            preMultiplayerScreen.SetActive(true);
            lobbyScreen.SetActive(false);
            loadingScreen.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed To Join Lobby With Code '{codeFromUI}': {e.Message}\n{e.StackTrace}");

            preMultiplayerScreen.SetActive(true);
            lobbyScreen.SetActive(false);
            loadingScreen.SetActive(false);
        }
    }

    public void OnStartGameButtonPressed()
    {
        if (!isHost) return;

        if (networkManager.IsServer)
        {
            //Load Game Scene
            networkManager.SceneManager.LoadScene("Multiplayer Game Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    #endregion

    void Update()
    {
        if (networkManager != null && networkManager.IsServer && networkManager.ConnectedClientsList != null)
        {
            int playerCount = networkManager.ConnectedClientsList.Count;
            playersWaitingText.text = $"Players Waiting: {playerCount} / {maxPlayers}";

            //Enable Start Game Button When Minimum Players Joined
            startGameButton.interactable = playerCount >= minPlayersToStart;
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

            if (!isHost)
            {
                //Update Join Code Text On Client
                if (currentLobby.Data.TryGetValue("joinCode", out var joinCodeData))
                {
                    joinCodeText.text = joinCodeData.Value;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to refresh lobby data: " + e.Message);
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
            Debug.LogWarning($"Unity Services already initialized or failed to initialize: {e.Message}");
        }
    }

    async System.Threading.Tasks.Task SignInAnonymouslyAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously as {AuthenticationService.Instance.PlayerId}");
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
