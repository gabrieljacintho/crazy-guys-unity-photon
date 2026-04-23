using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.GameManagement
{
    public class GameStateEvent : MonoBehaviour
    {
        [SerializeField] private GameState _gameState;
        [ShowIf("@_gameState == GameState.InGame")]
        [SerializeField] private bool _gameStarted;
        [ShowIf("@_gameState == GameState.InGame && !_gameStarted")]
        [SerializeField] private bool _gameNotStarted;

        private int _enter = 0;

        [Space]
        public UnityEvent OnEnter;
        public UnityEvent OnExit;


        private void OnEnable()
        {
            GameManager.GameStateChanged += OnGameStateChanged;
            OnGameStateChanged(GameManager.CurrentGameState);
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == _gameState)
            {
                if (_enter != 1)
                {
                    if (_gameState != GameState.InGame)
                    {
                        _enter = 1;
                        OnEnter?.Invoke();
                    }
                    else if (_gameStarted)
                    {
                        if (GameManager.LastGameState == GameState.NotStarted)
                        {
                            _enter = 1;
                            OnEnter?.Invoke();
                        }
                    }
                    else if (_gameNotStarted)
                    {
                        if (GameManager.LastGameState != GameState.NotStarted)
                        {
                            _enter = 1;
                            OnEnter?.Invoke();
                        }
                    }
                    else
                    {
                        _enter = 1;
                        OnEnter?.Invoke();
                    }
                }
            }
            else if (_enter != 0)
            {
                _enter = 0;
                OnExit?.Invoke();
            }
        }
    }
}