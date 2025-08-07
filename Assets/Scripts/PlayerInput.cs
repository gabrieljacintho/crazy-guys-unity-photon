using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = UnityEngine.Input;

namespace GabrielBertasso
{
    public class PlayerInput : QuantumEntityViewComponent
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

            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
            QuantumEvent.Subscribe<EventResetLookRotation>(this, OnResetLookRotation);
        }

        public override void OnUpdateView()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _input.MoveDirection = default;
                return;
            }

            var lookRotationDelta = new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));

#if UNITY_WEBGL && !UNITY_EDITOR
            if (Mathf.Abs(lookRotationDelta.x) > 45 || Mathf.Abs(lookRotationDelta.y) > 45)
            {
                // Prevent glitch in Chrome with high polling mice where cursor jumps rapidly from time to time
                lookRotationDelta = default;
            }

            // Sensitivity in WebGL builds on desktop is much higher for some reason, decrease it
            lookRotationDelta *= 0.5f;
#endif

            _input.LookRotation = ClampLookRotation(_input.LookRotation + lookRotationDelta.ToFPVector2());

            var moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _input.MoveDirection = moveDirection.normalized.ToFPVector2();

            _input.Jump = Input.GetButton("Fire1");
            _input.Sprint = Input.GetButton("Sprint");
        }
        
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