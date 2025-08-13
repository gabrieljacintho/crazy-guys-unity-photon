using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Input = UnityEngine.Input;

namespace GabrielBertasso
{
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
        [SerializeField] private CanvasGroup _connectGroup;
        [SerializeField] private CanvasGroup _disconnectGroup;
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private TextMeshProUGUI _statusText;

        private RealtimeClient _client;
        private CancellationTokenSource _cancellationTokenSource;

        private static string s_shutdownStatus;

        private bool IsConnected => QuantumRunner.Default != null;


        private async void OnEnable()
        {
            LoadNicknameText();
            await PreloadAllAssets();
            LoadStatusText();
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
                _connectGroup.gameObject.SetActive(!isConnectingOrConnected);
                _disconnectGroup.gameObject.SetActive(isConnectingOrConnected);
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

                // Force local mode in editor for faster testing
                var mode = _forceLocalMode && Application.isEditor ? DeterministicGameMode.Local : DeterministicGameMode.Multiplayer;

                if (mode == DeterministicGameMode.Multiplayer)
                {
                    _statusText.text = "Connecting to a room...";

                    _client = await ConnectToRoom(cancellationToken);
                    _client.CallbackMessage.Listen<MultiplayerPanel, OnDisconnectedMsg>(this, OnDisconnectedMessage);
                }

                _statusText.text = "Starting game session...";

                var runner = (QuantumRunner)await StartSession(mode, cancellationToken);
                runner.Game.AddPlayer(0, _runtimePlayer);

                _statusText.text = string.Empty;
                _panelGroup.gameObject.SetActive(false);
            }
            catch (Exception exception)
            {
                s_shutdownStatus = $"Connection failed: {exception.Message}";
                Debug.LogWarning(exception);

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

        private async Task<RealtimeClient> ConnectToRoom(CancellationToken cancellationToken)
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

            return await MatchmakingExtensions.ConnectToRoomAsync(matchmakingArguments);
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
                s_shutdownStatus = $"Disconnection failed: {exception.Message}";
                Debug.LogException(exception);
            }

            StartCoroutine(ReloadActiveScene());
        }

        public async void InvokeDisconnect()
        {
            await Disconnect();
        }

        #endregion

        #region Load

        private void LoadNicknameText()
        {
            var nickname = PlayerPrefs.GetString("player-name");
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = "Player" + Guid.NewGuid().ToString();
            }

            _nicknameInputField.text = nickname;
        }

        private void LoadStatusText()
        {
            _statusText.text = !string.IsNullOrEmpty(s_shutdownStatus) ? s_shutdownStatus : string.Empty;
            s_shutdownStatus = string.Empty;
        }

        private async Task PreloadAllAssets()
        {
            _statusText.text = "Preloading assets...";
            _connectGroup.interactable = false;

            var addressableAssets = QuantumUnityDB.Global.Entries
                .Where(x => x.Source is QuantumAssetObjectSourceAddressable)
                .Select(x => (x.Guid, ((QuantumAssetObjectSourceAddressable)x.Source).RuntimeKey));

            // preload all the addressable assets
            foreach (var (assetRef, address) in addressableAssets)
            {
                // there are a few ways to load an asset with Addressables (by label, by IResourceLocation, by address etc.)
                // but it seems that they're not fully interchangeable, i.e. loading by label will not make loading by address
                // be reported as done immediately; hence the only way to preload an asset for Quantum is to replicate
                // what it does internally, i.e. load with the very same parameters
                await Addressables.LoadAssetAsync<UnityEngine.Object>(address).Task;
            }

            _statusText.text = string.Empty;
            _connectGroup.interactable = true;
        }

        private IEnumerator ReloadActiveScene()
        {
            _panelGroup.gameObject.SetActive(false);
            var handle = Addressables.LoadSceneAsync(_scene);
            yield return handle;
            Debug.Log("Scene reloaded.");
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

            StartCoroutine(ReloadActiveScene());
        }
    }
}