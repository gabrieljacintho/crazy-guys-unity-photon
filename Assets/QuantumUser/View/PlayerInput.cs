using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = UnityEngine.Input;

namespace GabrielBertasso
{
    public class PlayerInput : QuantumEntityViewComponent
    {
        [SerializeField] private FPVector2 _pitchClamp = new FPVector2(-30, 70);
#if UNITY_ANDROID || UNITY_IOS
        [Header("Inputs")]
        [SerializeField] private RectTransform _moveInitialTransform;
        [SerializeField] private RectTransform _moveCurrentTransform;
        [SerializeField] private Vector2 _canvasSize = new Vector2(1920, 1080);
        [SerializeField] private float _moveMaxRadius = 20f;

        private Vector2 _lookRotationDelta;
        private Vector2 _moveDirection;
        private Vector2 _resolution;

        public bool IsJumpButtonPressed { get; set; }
        private bool IsSprintButtonPressed { get; set; }
#else
        [Header("Inputs")]
        [SerializeField] private string _lookXAxisName = "Mouse X";
        [SerializeField] private string _lookYAxisName = "Mouse Y";
        [SerializeField] private string _moveXAxisName = "Horizontal";
        [SerializeField] private string _moveYAxisName = "Vertical";
        [SerializeField] private string _jumpButtonName = "Jump";
        [SerializeField] private string _sprintButtonName = "Sprint";
#endif
        private Quantum.Input _input;

        public Vector2 LookRotation => _input.LookRotation.ToUnityVector2();

#if UNITY_ANDROID || UNITY_IOS
        private void Update()
        {
            ResetMoveDirection();
            if (Input.touchCount == 0)
            {
                return;
            }

            _resolution = new Vector2(Screen.width, Screen.height);

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (IsTouchingMoveButton(touch))
                {
                    UpdateMoveDirection(touch);
                }
                else
                {
                    UpdateLookRotationDelta(touch);
                }
            }
        }
#endif
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

        private Vector2 GetLookRotationDelta()
        {
#if UNITY_ANDROID || UNITY_IOS
            return _lookRotationDelta;
#else
            Vector2 value = new Vector2(-Input.GetAxisRaw(_lookYAxisName), Input.GetAxisRaw(_lookXAxisName));
    #if !UNITY_EDITOR
            if (Mathf.Abs(value.x) > 45 || Mathf.Abs(value.y) > 45)
            {
                // Prevent glitch in Chrome with high polling mice where cursor jumps rapidly from time to time
                value = default;
            }

            // Sensitivity in WebGL builds on desktop is much higher for some reason, decrease it
            value *= 0.5f;
    #endif
#endif
            return value;
        }

        private Vector2 GetMoveDirection()
        {
#if UNITY_ANDROID || UNITY_IOS
            return _moveDirection;
#else
            return new Vector2(Input.GetAxisRaw(_moveXAxisName), Input.GetAxisRaw(_moveYAxisName));
#endif
        }

        private bool CanJump()
        {
#if UNITY_ANDROID || UNITY_IOS
            return IsJumpButtonPressed;
#else
            return Input.GetButton(_jumpButtonName);
#endif
        }

        private bool CanSprint()
        {
#if UNITY_ANDROID || UNITY_IOS
            return IsSprintButtonPressed;
#else
            return Input.GetButton(_sprintButtonName);
#endif
        }

        #endregion

#if UNITY_ANDROID || UNITY_IOS
        private void UpdateLookRotationDelta(Touch touch)
        {
            _lookRotationDelta = GetCanvasPosition(touch.deltaPosition);
        }

        private void UpdateMoveDirection(Touch touch)
        {
            Vector2 direction = GetCanvasPosition(touch.position) - GetCanvasPosition(touch.rawPosition);
            _moveDirection = direction / _moveMaxRadius;

            _moveInitialTransform.gameObject.SetActive(true);
            _moveCurrentTransform.anchoredPosition = Vector2.ClampMagnitude(direction, _moveMaxRadius);
        }

        private void ResetMoveDirection()
        {
            _moveDirection = default;
            _moveInitialTransform.gameObject.SetActive(false);
        }

        private bool IsTouchingMoveButton(Touch touch)
        {
            Vector2 rawPosition = GetCanvasPosition(touch.rawPosition);
            Vector2 movePosition = GetCanvasPosition(_moveInitialTransform.anchoredPosition);
            return (rawPosition - movePosition).magnitude <= _moveMaxRadius;
        }

        private Vector2 GetCanvasPosition(Vector2 screenPosition)
        {
            return screenPosition / _resolution * _canvasSize;
        }
#endif

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