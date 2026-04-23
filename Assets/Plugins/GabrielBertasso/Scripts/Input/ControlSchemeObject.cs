using UnityEngine;

namespace GabrielBertasso.Input
{
    public class ControlSchemeObject : MonoBehaviour
    {
        [SerializeField] private GameObject _keyboardMouseObject;
        [SerializeField] private GameObject _xboxControllerObject;
        [SerializeField] private GameObject _dualshockControllerObject;


        private void OnEnable()
        {
            InputManager.ControlSchemeChanged += UpdateObject;
            UpdateObject(InputManager.CurrentControlScheme);
        }

        private void OnDisable()
        {
            InputManager.ControlSchemeChanged -= UpdateObject;
        }

        private void UpdateObject(ControlScheme controlScheme)
        {
            SetActive(false);

            switch (controlScheme)
            {
                case ControlScheme.KeyboardMouse when _keyboardMouseObject != null:
                    _keyboardMouseObject.SetActive(true);
                    break;

                case ControlScheme.XboxController when _xboxControllerObject != null:
                    _xboxControllerObject.SetActive(true);
                    break;

                case ControlScheme.PlayStationController when _dualshockControllerObject != null:
                    _dualshockControllerObject.SetActive(true);
                    break;
            }
        }

        private void SetActive(bool value)
        {
            if (_keyboardMouseObject != null)
            {
                _keyboardMouseObject.SetActive(value);
            }

            if (_xboxControllerObject != null)
            {
                _xboxControllerObject.SetActive(value);
            }

            if (_dualshockControllerObject != null)
            {
                _dualshockControllerObject.SetActive(value);
            }
        }
    }
}