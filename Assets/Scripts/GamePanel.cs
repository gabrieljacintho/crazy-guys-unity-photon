using Quantum;
using TMPro;
using UnityEngine;

namespace GabrielBertasso
{
    public class GamePanel : QuantumSceneViewComponent<SceneContext>
    {
        [SerializeField] private CanvasGroup _panelGroup;
        [SerializeField] private TextMeshProUGUI _instructionsText;
        [SerializeField] private TextMeshProUGUI _collectedCoinCountText;
        [SerializeField] private TextMeshProUGUI _winnerText;

        private PlayerRef _winner;


        public override void OnUpdateView()
        {
            var frame = PredictedFrame;
            if (frame == null || !frame.Exists(ViewContext.LocalPlayerEntity))
            {
                SetActivePanel(false);
                return;
            }

            var gameManager = frame.GetSingleton<GameManager>();
            var player = frame.Get<Player>(ViewContext.LocalPlayerEntity);

            SetActivePanel(true);
            UpdateWinnerText(frame, gameManager);
            UpdateCoinCountText(player);
            UpdateInstructionsText(gameManager, player);
        }

        private void SetActivePanel(bool value)
        {
            _panelGroup.gameObject.SetActive(value);
        }

        private void UpdateWinnerText(Frame frame, GameManager gameManager)
        {
            if (_winner == gameManager.Winner)
            {
                return;
            }

            _winner = gameManager.Winner;

            var winnerData = frame.GetPlayerData(_winner);
            _winnerText.text = winnerData != null ? $"The winner is {winnerData.PlayerNickname}!" : string.Empty;
        }

        private void UpdateCoinCountText(Player player)
        {
            _collectedCoinCountText.text = $"x{player.CollectedCoinCount}";
        }

        private void UpdateInstructionsText(GameManager gameManager, Player player)
        {
            int remainingCoinsCount = gameManager.MinCoinsToWin - player.CollectedCoinCount;
            _instructionsText.text = remainingCoinsCount <= 0 ? "Run to the TOP!" : $"Collect {remainingCoinsCount} coins";
        }
    }
}
