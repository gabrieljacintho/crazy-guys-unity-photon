using UnityEngine;

namespace GabrielBertasso
{
    public class TouchPlayerInput : PlayerInput
    {
        [SerializeField] private RectTransform _moveInitialTransform;
        [SerializeField] private RectTransform _moveCurrentTransform;
        [SerializeField] private Vector2 _canvasSize = new Vector2(1920, 1080);
        [SerializeField] private float _moveMaxRadius = 20f;

        private Vector2 _lookRotationDelta;
        private Vector2 _moveDirection;
        private Vector2 _resolution;

        public bool IsJumpButtonPressed { get; set; }
        private bool IsSprintButtonPressed { get; set; }


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

        private void UpdateMoveDirection(Touch touch)
        {
            Vector2 direction = GetCanvasPosition(touch.position) - GetCanvasPosition(touch.rawPosition);
            _moveDirection = direction / _moveMaxRadius;

            _moveInitialTransform.gameObject.SetActive(true);
            _moveCurrentTransform.anchoredPosition = Vector2.ClampMagnitude(direction, _moveMaxRadius);
        }

        private void UpdateLookRotationDelta(Touch touch)
        {
            _lookRotationDelta = GetCanvasPosition(touch.deltaPosition);
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

        #region Inputs

        protected override Vector2 GetLookRotationDelta()
        {
            return _lookRotationDelta;
        }

        protected override Vector2 GetMoveDirection()
        {
            return _moveDirection;
        }

        protected override bool CanJump()
        {
            return IsJumpButtonPressed;
        }

        protected override bool CanSprint()
        {
            return IsSprintButtonPressed;
        }

        #endregion
    }
}