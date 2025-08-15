using GabrielBertasso.Helpers;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabrielBertasso
{
    public class PlayerInput : QuantumEntityViewComponent
    {
        [SerializeField] private FPVector2 _pitchClamp = new FPVector2(-30, 70);
        [SerializeField] private FP _lookSensitivity = FP._0_20;

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        [Header("Keyboard, Mouse and Gamepad")]
        [SerializeField] private InputActionReference _lookInput;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private InputActionReference _jumpInput;
#endif
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        [Header("Touch")]
        [SerializeField] private GameObject _touchInputContent;
        [SerializeField] private RectTransform _touchMoveInitialTransform;
        [SerializeField] private RectTransform _touchMoveCurrentTransform;

        private Vector2 _screenResolution;
        private Vector2 _canvasResolution;
        private float _touchMoveCurrentTransformMaxRadius;

        private Vector2 _lookRotationDelta;
        private Vector2 _moveDirection;
        private Vector2 _lastLookTouchCanvasPosition;

        public bool IsJumpButtonPressed { get; set; }
        private bool IsSprintButtonPressed { get; set; }
#endif
        private Quantum.Input _input;

        public Vector2 LookRotation => _input.LookRotation.ToUnityVector2();

#if UNITY_ANDROID || UNITY_IOS
        private void Awake()
        {
            _touchInputContent.SetActive(true);
            _screenResolution = new Vector2(Screen.width, Screen.height);
            _canvasResolution = _touchInputContent.GetComponentInChildren<CanvasScaler>().referenceResolution;
            _touchMoveCurrentTransformMaxRadius = _touchMoveInitialTransform.rect.width / 2f;
            EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            ResetMoveDirection();

            foreach (var finger in Touch.activeFingers)
            {
                Touch touch = finger.currentTouch;
                if (!touch.isInProgress)
                {
                    continue;
                }

                if (WasTouchingMoveButton(touch))
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
            if (!SymbolsHelper.IsInMobilePlatform() && Cursor.lockState != CursorLockMode.Locked)
            {
                _input.MoveDirection = default;
                return;
            }

            _input.LookRotation = ClampLookRotation(_input.LookRotation + GetLookRotationDelta().ToFPVector2());
            _input.MoveDirection = Vector2.ClampMagnitude(GetMoveDirection(), 1f).ToFPVector2();
            _input.Jump = CanJump();
        }

        #region Inputs

        private Vector2 GetLookRotationDelta()
        {
#if UNITY_ANDROID || UNITY_IOS
            return _lookRotationDelta * _lookSensitivity.AsFloat;
#else
            Vector2 value = _lookInput.action.ReadValue<Vector2>();
            value = new Vector2(-value.y, value.x) * _lookSensitivity.AsFloat;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Mathf.Abs(value.x) > 45 || Mathf.Abs(value.y) > 45)
            {
                // Prevent glitch in Chrome with high polling mice where cursor jumps rapidly from time to time
                value = default;
            }

            // Sensitivity in WebGL builds on desktop is much higher for some reason, decrease it
            value *= 0.5f;
#endif
            return value;
#endif
        }

        private Vector2 GetMoveDirection()
        {
#if UNITY_ANDROID || UNITY_IOS
            return _moveDirection;
#else
            return _moveInput.action.ReadValue<Vector2>();
#endif
        }

        private bool CanJump()
        {
#if UNITY_ANDROID || UNITY_IOS
            return IsJumpButtonPressed;
#else
            return _jumpInput.action.WasPressedThisFrame();
#endif
        }

        #endregion

#if UNITY_ANDROID || UNITY_IOS
        private void UpdateLookRotationDelta(Touch touch)
        {
            Vector2 position = GetCanvasPosition(touch.screenPosition);

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _lastLookTouchCanvasPosition = position;
            }

            Vector2 input = position - _lastLookTouchCanvasPosition;
            _lookRotationDelta = new Vector2(-input.y, input.x);

            _lastLookTouchCanvasPosition = position;
        }

        private void UpdateMoveDirection(Touch touch)
        {
            Vector2 direction = GetCanvasPosition(touch.screenPosition) - _touchMoveInitialTransform.anchoredPosition;
            _moveDirection = direction / _touchMoveCurrentTransformMaxRadius;

            _touchMoveCurrentTransform.anchoredPosition = Vector2.ClampMagnitude(direction, _touchMoveCurrentTransformMaxRadius);
        }

        private void ResetMoveDirection()
        {
            _moveDirection = default;
            _touchMoveCurrentTransform.anchoredPosition = Vector2.zero;
        }

        private bool WasTouchingMoveButton(Touch touch)
        {
            Vector2 touchCanvasPosition = GetCanvasPosition(touch.startScreenPosition);
            return (touchCanvasPosition - _touchMoveInitialTransform.anchoredPosition).magnitude <= _touchMoveCurrentTransformMaxRadius;
        }

        private Vector2 GetCanvasPosition(Vector2 screenPosition)
        {
            return screenPosition / _screenResolution * _canvasResolution;
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