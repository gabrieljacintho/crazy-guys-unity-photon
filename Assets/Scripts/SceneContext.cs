using Quantum;
using UnityEngine;

namespace GabrielBertasso
{
    public class SceneContext : MonoBehaviour, IQuantumViewContext
    {
        [SerializeField] private PlayerRef _localPlayer;
        [SerializeField] private EntityRef _localPlayerEntity;

        public PlayerRef LocalPlayer
        {
            get => _localPlayer;
            set => _localPlayer = value;
        }
        public EntityRef LocalPlayerEntity
        {
            get => _localPlayerEntity;
            set => _localPlayerEntity = value;
        }
    }
}