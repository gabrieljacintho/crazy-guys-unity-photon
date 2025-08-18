# Crazy Guys

Crazy Guys is a multiplayer party royale game prototype inspired by Stumble Guys, and developed for Photon Engine study with Unity Engine, C# and Photon Engine (Quantum 3).

# Setup
1. Download [Photon Quantum SDK](https://www.photonengine.com/sdks#quantum)
2. Import Photon Quantum SDK into Unity project
3. Create a new App on Photon Engine Dashboard
4. Register the App Id on Quantum SDK (Tools/Quantum/Quantum Hub/Installation)
5. Add the following scripts to the game scene: QuantumEntityViewUpdater, QuantumMapData and QuantumRunnerLocalDebug (optional).
6. (Optional) Add the QuantumStats prefab (located in Assets/Photon/Quantum/Resources) to the game scene.

# Room
## How to join or create a room?
```
public class MultiplayerPanel : MonoBehaviour
{
    private RealtimeClient _client;
    private CancellationTokenSource _cancellationTokenSource;

    private bool IsConnected => QuantumRunner.Default != null;

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
}
```

## How to leave a room?
```
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
```

# Session
## How to create a lobby?

## How to start a session (exit the lobby)?

## How to end a session (by victory or defeat)?

## How to create a new session (return to the lobby)?

# Player
## How to set player nickname?
```
private void SetPlayerNickname(string value)
{
    PlayerPrefs.SetString("player-name", value);
    _runtimePlayer.PlayerNickname = value;
}
```

## How to spawn player character?
```
[Preserve]
public unsafe class GameplaySystem : SystemSignalsOnly, ISignalOnPlayerAdded
{
    public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
    {
        RuntimePlayer runtimePlayer = frame.GetPlayerData(player);
        EntityRef playerEntity = frame.Create(runtimePlayer.PlayerAvatar);

        frame.AddOrGet<PlayerLink>(playerEntity, out var playerLink);
        playerLink->PlayerRef = player;

        RespawnPlayer(frame, playerEntity);
    }

    private void RespawnPlayer(Frame frame, EntityRef playerEntity)
    {
        var spawnData = GetSpawnData(frame);
        var kcc = frame.Unsafe.GetPointer<KCC>(playerEntity);
        kcc->Teleport(frame, spawnData.Position);
        kcc->SetLookRotation(spawnData.Rotation);
        kcc->Data.KinematicVelocity = default;
    }

    private (FPVector3 Position, FPQuaternion Rotation) GetSpawnData(Frame frame)
    {
        var gameManager = frame.Unsafe.GetPointerSingleton<GameManager>();

        var position = gameManager->SpawnPosition + frame.RNG->InUnitCircle(true).XOY * gameManager->SpawnRadius;
        var rotation = FPQuaternion.Euler(0, 0, 0);

        return (position, rotation);
    }
}
```

## How to control player character?

# User Interface
## How to update the user interface?
```
public class GamePanel : QuantumSceneViewComponent<SceneContext>
{
    [SerializeField] private TextMeshProUGUI _collectedCoinCountText;

    public override void OnUpdateView()
    {
        var frame = PredictedFrame;
        if (frame == null || !frame.Exists(ViewContext.LocalPlayerEntity))
        {
            return;
        }

        var player = frame.Get<Player>(ViewContext.LocalPlayerEntity);
        UpdateCoinCountText(player);
    }

    private void UpdateCoinCountText(Player player)
    {
        _collectedCoinCountText.text = $"x{player.CollectedCoinCount}";
    }
}
```

# Audio
## How to play/stop an audio clip?
```
public class PlayerView : QuantumEntityViewComponent<SceneContext>
{
    [SerializeField] private AudioClip _jumpAudioClip;

    public override void OnActivate(Frame frame)
    {
        QuantumEvent.Subscribe<EventJumped>(this, OnJumped);
    }

    private void OnJumped(EventJumped callback)
    {
        if (callback.Entity != EntityRef)
        {
            return;
        }
    
        AudioSource.PlayClipAtPoint(_jumpAudioClip, transform.position, 1f);
    }
}
```

# Particles
## How to play/stop particles?
```
public class PlayerView : QuantumEntityViewComponent<SceneContext>
{
    [SerializeField] private ParticleSystem _dustParticles;

    public override void OnUpdateView()
    {
        var kcc = GetPredictedQuantumComponent<KCC>();
        UpdateDustParticles(kcc);
    }
    
    private void UpdateDustParticles(KCC kcc)
    {
        var emission = _dustParticles.emission;
        emission.enabled = kcc.IsGrounded && kcc.RealSpeed > 1;
    }
}
```
