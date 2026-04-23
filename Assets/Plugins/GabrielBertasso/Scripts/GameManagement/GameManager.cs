using System;
using GabrielBertasso.Patterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.GameManagement
{
    [Serializable]
    public enum GameState
    {
        NotStarted,
        InGame,
        Interaction,
        Paused,
        Over,
        Win
    }

    public class GameManager :
#if GABRIEL_BERTASSO_SAVE
        SaveSingleton<GameManager>
#else
        Singleton<GameManager>
#endif
    {
        [Header("Game Manager")]
        [SerializeField] private GameState _initialState;

        private static float s_targetTimeScale = 1f;
        [ShowInInspector, ReadOnly] private static GameState s_currentGameState = GameState.NotStarted;
        private static GameState s_lastGameState;

        public static float TargetTimeScale
        {
            get => s_targetTimeScale;
            set
            {
                s_targetTimeScale = value;
                if (s_currentGameState != GameState.Paused)
                {
                    Time.timeScale = value;
                }
            }
        }
        public static GameState CurrentGameState
        {
            get => s_currentGameState;
            set => ChangeGameState(value);
        }
        public static GameState LastGameState => s_lastGameState;

        public static bool InGame => s_currentGameState == GameState.InGame;
        public static bool InAnyGameState => s_currentGameState == GameState.InGame || s_currentGameState == GameState.Interaction;
        public static bool IsPaused
        {
            get => s_currentGameState == GameState.Paused;
            set => SetPaused(value);
        }

        public delegate void GameStateChangedEvent(GameState gameState);
        public static event GameStateChangedEvent GameStateChanged;

        public delegate void GameStateEvent();
        public static event GameStateEvent GameStarted;
        public static event GameStateEvent GamePaused;
        public static event GameStateEvent GameUnpaused;
        public static event GameStateEvent GameOver;
        public static event GameStateEvent GameWon;
        public static event GameStateEvent GameReset;


        protected override void Awake()
        {
            base.Awake();
            if (Instance != this)
            {
                return;
            }

            TargetTimeScale = 1f;
            CurrentGameState = _initialState;
        }

        [Button]
        public static void ChangeGameState(GameState newGameState)
        {
            if (s_currentGameState == newGameState)
            {
                return;
            }

            s_lastGameState = s_currentGameState;
            s_currentGameState = newGameState;

            if (s_currentGameState == GameState.NotStarted)
            {
                TargetTimeScale = 1f;
            }

            switch (s_currentGameState)
            {
                case GameState.Paused:
                case GameState.Over:
                case GameState.Win:
                    Time.timeScale = 0f;
                    break;

                default:
                    Time.timeScale = TargetTimeScale;
                    break;
            }

            switch (s_lastGameState)
            {
                case GameState.Paused:
                    GameUnpaused?.Invoke();
                    break;
            }

            switch (s_currentGameState)
            {
                case GameState.NotStarted:
                    TargetTimeScale = 1f;
                    GameReset?.Invoke();
                    break;

                case GameState.InGame:
                    if (s_lastGameState == GameState.NotStarted)
                    {
                        GameStarted?.Invoke();
                    }
                    break;

                case GameState.Paused:
                    GamePaused?.Invoke();
                    break;

                case GameState.Over:
                    GameOver?.Invoke();
                    break;

                case GameState.Win:
                    GameWon?.Invoke();
                    break;
            }

            GameStateChanged?.Invoke(s_currentGameState);

            Debug.Log("Game state changed: \"" + s_currentGameState + "\"\n" + System.Environment.StackTrace);
        }

        public static void RestartGame()
        {
            switch (s_currentGameState)
            {
                case GameState.Paused:
                    GameUnpaused?.Invoke();
                    break;
            }

            s_currentGameState = GameState.NotStarted;
            ChangeGameState(GameState.InGame);
        }

        public static void SetPaused(bool value)
        {
            if (value)
            {
                ChangeGameState(GameState.Paused);
            }
            else if (s_currentGameState == GameState.Paused)
            {
                ChangeGameState(s_lastGameState);
            }
        }

        [Button]
        public static void TogglePause() => IsPaused = !IsPaused;

#if GABRIEL_BERTASSO_SAVE
        #region Save

        protected override void SaveOperation(ES3Settings settings)
        {
            ES3.Save(SaveKey, CurrentGameState, settings);
        }

        protected override void LoadOperation(ES3Settings settings)
        {
            CurrentGameState = ES3.Load<GameState>(SaveKey, settings);
        }

        protected override void LoadDefaultValues()
        {
            CurrentGameState = _initialState;
        }

        #endregion
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }
    }
}
