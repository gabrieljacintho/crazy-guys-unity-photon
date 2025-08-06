using System;
using System.Threading;
using System.Threading.Tasks;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Input = UnityEngine.Input;

[Serializable]
public class AssetReferenceScene : AssetReference
{
    public AssetReferenceScene(string guid) : base(guid) { }

    public override bool ValidateAsset(string path)
    {
        return path.EndsWith(".unity");
    }
}

public class MultiplayerPanel : MonoBehaviour
{
    [SerializeField] private RuntimeConfig _runtimeConfig;
    [SerializeField] private RuntimePlayer _runtimePlayer;
    [SerializeField] private AssetReferenceScene _scene;

    [Header("Debug")]
    [Tooltip("For debug purposes it is possible to force single-player game (starts faster)")]
    [SerializeField] private bool _forceLocalMode;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _panelGroup;
    [SerializeField] private GameObject _connectGroup;
    [SerializeField] private GameObject _disconnectGroup;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TextMeshProUGUI _statusText;

    private RealtimeClient _client;
    private CancellationTokenSource _cancellationTokenSource;

    private static string s_shutdownStatus;

    private bool IsConnected => QuantumRunner.Default != null;


    private void OnEnable()
    {
        var nickname = PlayerPrefs.GetString("player-name");
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Player" + Guid.NewGuid().ToString();
        }

        _nicknameInputField.text = nickname;

        _statusText.text = !string.IsNullOrEmpty(s_shutdownStatus) ? s_shutdownStatus : string.Empty;
        s_shutdownStatus = string.Empty;
    }

    private void Update()
    {
        if (IsConnected)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePanelVisibility();
            }
        }

        if (_panelGroup.gameObject.activeSelf)
        {
            bool isConnectingOrConnected = _cancellationTokenSource != null || IsConnected;
            _connectGroup.SetActive(!isConnectingOrConnected);
            _disconnectGroup.SetActive(isConnectingOrConnected);
            _nicknameInputField.interactable = !isConnectingOrConnected;
            _roomNameInputField.interactable = !isConnectingOrConnected;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    #region Connect

    public async Task Connect()
    {
        try
        {
            if (_client != null)
            {
                await Disconnect();
            }

            SetPlayerNickname(_nicknameInputField.text);

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var mode = _forceLocalMode && Application.isEditor ? DeterministicGameMode.Local : DeterministicGameMode.Multiplayer;

            if (mode == DeterministicGameMode.Multiplayer)
            {
                _statusText.text = "Connecting to a room...";
                await ConnectToRoom(cancellationToken);
            }

            _statusText.text = "Starting game session...";
            var runner = (QuantumRunner)await StartSession(mode, cancellationToken);

            AddPlayer(runner);

            _statusText.text = string.Empty;
            _panelGroup.gameObject.SetActive(false);
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception);
            _statusText.text = $"Connection failed: {exception.Message}";

            await Disconnect();
        }

        _cancellationTokenSource = null;
    }

    public async void InvokeConnect()
    {
        await Connect();
    }

    private void SetPlayerNickname(string value)
    {
        PlayerPrefs.SetString("player-name", value);
        _runtimePlayer.PlayerNickname = value;
    }

    private async Task ConnectToRoom(CancellationToken cancellationToken)
    {
        var matchmakingArguments = new MatchmakingArguments
        {
            PhotonSettings = new AppSettings(PhotonServerSettings.Global.AppSettings),
            RoomName = !string.IsNullOrEmpty(_roomNameInputField.text) ? _roomNameInputField.text : null,
            PluginName = "QuantumPlugin",
            MaxPlayers = Quantum.Input.MAX_COUNT,
        };

        if (matchmakingArguments.AsyncConfig == null)
        {
            matchmakingArguments.AsyncConfig = AsyncConfig.Global;
            matchmakingArguments.AsyncConfig.CancellationToken = cancellationToken;
        }

        _client = await MatchmakingExtensions.ConnectToRoomAsync(matchmakingArguments);
        _client.CallbackMessage.Listen<MultiplayerPanel, OnDisconnectedMsg>(this, OnDisconnectedMessage);
    }

    private async Task<SessionRunner> StartSession(DeterministicGameMode mode, CancellationToken cancellationToken)
    {
        var sessionRunnerArguments = new SessionRunner.Arguments
        {
            RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
            GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
            ClientId = _client?.UserId,
            RuntimeConfig = _runtimeConfig,
            SessionConfig = QuantumDeterministicSessionConfigAsset.DefaultConfig,
            GameMode = mode,
            PlayerCount = Quantum.Input.MAX_COUNT,
            Communicator = _client != null ? new QuantumNetworkCommunicator(_client) : null,
            CancellationToken = cancellationToken,
        };

        return await SessionRunner.StartAsync(sessionRunnerArguments);
    }

    private void AddPlayer(QuantumRunner runner)
    {
        runner.Game.AddPlayer(0, _runtimePlayer);
    }

    #endregion

    #region Disconnect

    public async Task Disconnect()
    {
        try
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            _statusText.text = "Disconnecting...";
            _panelGroup.interactable = false;

            if (_client != null)
            {
                _client.CallbackMessage.UnlistenAll(this);
                _client = null;
            }

            await QuantumRunner.ShutdownAllAsync();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }

        ReloadActiveScene();
    }

    public async void InvokeDisconnect()
    {
        await Disconnect();
    }

    #endregion

    private void TogglePanelVisibility()
    {
        _panelGroup.gameObject.SetActive(!_panelGroup.gameObject.activeSelf);
    }

    private void OnDisconnectedMessage(OnDisconnectedMsg message)
    {
        s_shutdownStatus = $"Shutdown: {message.cause}";
        Debug.LogWarning(s_shutdownStatus);

        ReloadActiveScene();
    }

    private void ReloadActiveScene()
    {
        _panelGroup.gameObject.SetActive(false);
        Addressables.LoadSceneAsync(_scene);
    }
}
