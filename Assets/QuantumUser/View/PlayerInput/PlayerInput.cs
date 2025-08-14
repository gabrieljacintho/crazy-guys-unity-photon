using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace GabrielBertasso
{
    public abstract class PlayerInput : QuantumEntityViewComponent
    {
        [SerializeField] private FPVector2 _pitchClamp = new FPVector2(-30, 70);

        private Quantum.Input _input;

        public Vector2 LookRotation => _input.LookRotation.ToUnityVector2();


        public override void OnActivate(Frame frame)
        {
            var playerLink = GetPredictedQuantumComponent<PlayerLink>();
            if (!Game.PlayerIsLocal(playerLink.PlayerRef))
            {
                enabled = false;
                return;
            }

            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback), (DispatchableFilter)null, false, true);
            QuantumEvent.Subscribe<EventResetLookRotation>(this, OnResetLookRotation, (DispatchableFilter)null, false, true);
        }

        public override void OnDeactivate()
        {
            QuantumCallback.UnsubscribeListener(this);
        }

        public override void OnUpdateView()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _input.MoveDirection = default;
                return;
            }

            _input.LookRotation = ClampLookRotation(_input.LookRotation + GetLookRotationDelta().ToFPVector2());
            _input.MoveDirection = Vector2.ClampMagnitude(GetMoveDirection(), 1f).ToFPVector2();
            _input.Jump = CanJump();
            _input.Sprint = CanSprint();
        }

        #region Inputs

        protected abstract Vector2 GetLookRotationDelta();

        protected abstract Vector2 GetMoveDirection();

        protected abstract bool CanJump();

        protected abstract bool CanSprint();

        #endregion

        private void PollInput(CallbackPollInput callback)
        {
            callback.SetInput(_input, DeterministicInputFlags.Repeatable);
        }

        private void OnResetLookRotation(EventResetLookRotation callback)
        {
            _input.LookRotation = callback.Look;
        }

        private FPVector2 ClampLookRotation(FPVector2 lookRotation)
        {
            lookRotation.X = FPMath.Clamp(lookRotation.X, _pitchClamp.X, _pitchClamp.Y);
            return lookRotation;
        }
    }
}