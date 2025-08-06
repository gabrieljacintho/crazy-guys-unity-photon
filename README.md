# Crazy Guys

Crazy Guys is a multiplayer party royale game developed for Photon Engine study with Unity Engine, C# and Photon Engine (Quantum 3).

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

## How to control player character?

# User Interface
## How to update the user interface?

# Audio
## How to play/stop an audio clip?

# Particles
## How to play/stop particles?
