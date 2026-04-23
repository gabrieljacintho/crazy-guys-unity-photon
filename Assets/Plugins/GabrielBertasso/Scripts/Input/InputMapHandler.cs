using System;
using GabrielBertasso.Helpers;
using GabrielBertasso.Identities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabrielBertasso.Input
{
    public class InputMapHandler : MonoBehaviour
    {
        [Serializable]
        private enum ActiveMode
        {
            None,
            Enable,
            Disable
        }

        [SerializeField] private string _playerInputId;
        [SerializeField] private string _inputMapName;
        [SerializeField] private ActiveMode _activeModeOnEnableComponent = ActiveMode.Enable;
        [SerializeField] private ActiveMode _activeModeOnDisableComponent = ActiveMode.Disable;
        [SerializeField] private bool _developmentOnly;

        private InputActionMap _inputMap;
        private PlayerInput _playerInput;

        private PlayerInput PlayerInput
        {
            get
            {
                if (_playerInput == null)
                {
                    _playerInput = ComponentID.FindComponentWithID<PlayerInput>(_playerInputId, true);
                }

                return _playerInput;
            }
        }
        private InputActionMap InputMap
        {
            get
            {
                if (_inputMap == null && PlayerInput != null)
                {
                    _inputMap = PlayerInput.actions.FindActionMap(_inputMapName);
                }

                return _inputMap;
            }
        }


        private void OnEnable()
        {
            switch (_activeModeOnEnableComponent)
            {
                case ActiveMode.Enable:
                    Enable();
                    break;

                case ActiveMode.Disable:
                    Disable();
                    break;
            }
        }

        private void OnDisable()
        {
            switch (_activeModeOnDisableComponent)
            {
                case ActiveMode.Enable:
                    Enable();
                    break;

                case ActiveMode.Disable:
                    Disable();
                    break;
            }
        }

        public void Enable()
        {
            if (_developmentOnly && !SymbolsHelper.IsInDevelopment())
            {
                return;
            }

            InputMap?.Enable();
        }

        public void Disable()
        {
            if (_developmentOnly && !SymbolsHelper.IsInDevelopment())
            {
                return;
            }

            InputMap?.Disable();
        }
    }
}