using UnityEngine;

namespace GabrielBertasso.GameManagement
{
    public class ChangeGameState : MonoBehaviour
    {
        [SerializeField] private GameState _newGameState;
        [SerializeField] private bool _changeOnEnable;


        private void OnEnable()
        {
            if (_changeOnEnable)
            {
                Change();
            }
        }

        public void Change()
        {
            GameManager.ChangeGameState(_newGameState);
        }

        public void SetPaused(bool value)
        {
            GameManager.IsPaused = value;
        }

        public void TogglePause()
        {
            GameManager.TogglePause();
        }

        public void RestartGame()
        {
            GameManager.RestartGame();
        }
    }
}