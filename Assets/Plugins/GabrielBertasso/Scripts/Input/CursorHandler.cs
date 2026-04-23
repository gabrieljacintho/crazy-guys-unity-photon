using GabrielBertasso.Input;
using GabrielBertasso.Patterns;
using UnityEngine;

namespace GabrielBertasso
{
    public class CursorHandler : NonSingleton<CursorHandler>
    {
        [SerializeField] private bool _visible = true;
        [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Confined;


        protected override void OnEnable()
        {
            base.OnEnable();

            ActiveInstanceChanged += UpdateCursor;
            InputManager.ControlSchemeChanged += UpdateCursor;

            UpdateCursor();
        }

        private void Update()
        {
            UpdateCursor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            ActiveInstanceChanged -= UpdateCursor;
            InputManager.ControlSchemeChanged -= UpdateCursor;
        }

        private void UpdateCursor()
        {
            if (ActiveInstance != this)
            {
                return;
            }

            UpdateVisible();
            UpdateLockState();
        }

        private void UpdateCursor(CursorHandler cursorHandler) => UpdateCursor();

        private void UpdateCursor(ControlScheme controlScheme) => UpdateCursor();

        private void UpdateVisible()
        {
            bool newVisible = true;
            if (Application.isFocused)
            {
                newVisible = _visible && InputManager.CurrentControlScheme == ControlScheme.KeyboardMouse;
            }

            Cursor.visible = newVisible;
        }

        private void UpdateLockState()
        {
            CursorLockMode lockState = CursorLockMode.None;
            if (Application.isFocused)
            {
                lockState = _lockMode;
            }

            Cursor.lockState = lockState;
        }
    }
}