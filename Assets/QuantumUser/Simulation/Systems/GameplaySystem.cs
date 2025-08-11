using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.GabrielBertasso
{
    [Preserve]
    public unsafe class GameplaySystem : SystemMainThread, ISignalOnPlayerAdded, ISignalOnPlayerRemoved,
        ISignalPlayerFell, ISignalOnTrigger3D
    {
        public override void Update(Frame frame)
        {
            var gameManager = frame.Unsafe.GetPointerSingleton<GameManager>();

            if (gameManager->GameOverTimer.HasStoppedThisFrame(frame))
            {
                gameManager->GameOverTimer = default;
                gameManager->Winner = default;

                foreach (var pair in frame.Unsafe.GetComponentBlockIterator<Player>())
                {
                    pair.Component->CollectedCoinCount = 0;
                    RespawnPlayer(frame, pair.Entity);
                }

                // Enable player movement
                frame.SystemEnable<KCCSystem>();
            }
        }

        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            RuntimePlayer runtimePlayer = frame.GetPlayerData(player);
            EntityRef playerEntity = frame.Create(runtimePlayer.PlayerAvatar);

            frame.AddOrGet<PlayerLink>(playerEntity, out var playerLink);
            playerLink->PlayerRef = player;

            RespawnPlayer(frame, playerEntity);
        }

        public void OnPlayerRemoved(Frame frame, PlayerRef player)
        {
            foreach (var pair in frame.GetComponentIterator<PlayerLink>())
            {
                if (pair.Component.PlayerRef != player)
                {
                    continue;
                }

                frame.Destroy(pair.Entity);
            }
        }

        public void PlayerFell(Frame frame, EntityRef entity)
        {
            RespawnPlayer(frame, entity);
        }

        public void OnTrigger3D(Frame frame, TriggerInfo3D info)
        {
            if (!frame.Unsafe.TryGetPointer<Player>(info.Other, out var player))
            {
                return;
            }

            if (frame.Has<Coin>(info.Entity))
            {
                player->CollectedCoinCount++;

                frame.Signals.CoinCollected(info.Entity);
                frame.Events.CoinCollected(info.Entity, info.Other);
            }
            else if (frame.Has<Flag>(info.Entity))
            {
                CheckGameOver(frame, info.Other, player);
            }
        }

        private void RespawnPlayer(Frame frame, EntityRef playerEntity)
        {
            var spawnData = GetSpawnData(frame);
            var kcc = frame.Unsafe.GetPointer<KCC>(playerEntity);
            kcc->Teleport(frame, spawnData.Position);
            kcc->SetLookRotation(spawnData.Rotation);
            kcc->Data.KinematicVelocity = default;
        }

        private void CheckGameOver(Frame frame, EntityRef playerEntity, Player* player)
        {
            var gameManager = frame.Unsafe.GetPointerSingleton<GameManager>();

            if (gameManager->Winner.IsValid || player->CollectedCoinCount < gameManager->MinCoinsToWin)
            {
                return;
            }

            gameManager->Winner = frame.Unsafe.GetPointer<PlayerLink>(playerEntity)->PlayerRef;
            gameManager->GameOverTimer = FrameTimer.FromSeconds(frame, gameManager->GameOverTime);

            // Stop player movement
            frame.SystemDisable<KCCSystem>();
        }

        private (FPVector3 Position, FPQuaternion Rotation) GetSpawnData(Frame frame)
        {
            var gameManager = frame.Unsafe.GetPointerSingleton<GameManager>();

            var position = gameManager->SpawnPosition + frame.RNG->InUnitCircle(true).XOY * gameManager->SpawnRadius;
            var rotation = FPQuaternion.Euler(0, 0, 0);

            return (position, rotation);
        }
    }
}
