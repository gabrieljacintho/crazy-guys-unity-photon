using GabrielBertasso.Extensions;
using GabrielBertasso.GameManagement;
using GabrielBertasso.Helpers;
#if GABRIEL_BERTASSO_CROSS_PLATFORM
using GabrielBertasso.CrossPlatform;
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabrielBertasso.Input
{
    public abstract class InputComponent : MonoBehaviour
    {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
        [HideIf("_platformSpecific")]
#endif
        [SerializeField] private InputActionReference _inputReference;
#if GABRIEL_BERTASSO_CROSS_PLATFORM
        [HideIf("_platformSpecific")]
#endif
        [SerializeField] private InputAction _input;
#if GABRIEL_BERTASSO_CROSS_PLATFORM
        [ShowIf("_platformSpecific")]
        [SerializeField] PlatformValue<InputActionReference> _platformInputReference;
        [SerializeField] private bool _platformSpecific;
#endif
        [SerializeField] private bool _onlyInGame;
        [SerializeField] private bool _developmentOnly;


        protected virtual void Start()
        {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
            if (!_platformSpecific)
            {
                _input?.Enable();
            }
#else
            _input?.Enable();
#endif
        }

        protected virtual void OnEnable()
        {
            this.DoOnNextFrame(() =>
            {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
                if (_platformSpecific)
                {
                    RegisterPlatformSpecificInput();
                }
                else
#endif
                {
                    RegisterInput();
                }
            });
        }

        protected virtual void OnDisable()
        {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
            if (_platformSpecific)
            {
                UnregisterPlatformSpecificInput();
            }
            else
#endif
            {
                UnregisterInput();
            }
        }

        private void RegisterInput()
        {
            if (_inputReference != null)
            {
                _inputReference.action.Reset();
                _inputReference.action.performed += OnPerformFunc;
            }

            if (_input != null)
            {
                _input.Reset();
                _input.performed += OnPerformFunc;
            }
        }

        private void UnregisterInput()
        {
            if (_inputReference != null)
            {
                _inputReference.action.performed -= OnPerformFunc;
            }

            if (_input != null)
            {
                _input.performed -= OnPerformFunc;
            }
        }

#if GABRIEL_BERTASSO_CROSS_PLATFORM
        private void RegisterPlatformSpecificInput()
        {
            InputActionReference actionReference = _platformInputReference;
            if (actionReference != null)
            {
                actionReference.action.Reset();
                actionReference.action.performed += OnPerformFunc;
            }
        }

        private void UnregisterPlatformSpecificInput()
        {
            InputActionReference actionReference = _platformInputReference;
            if (actionReference != null)
            {
                actionReference.action.performed -= OnPerformFunc;
            }
        }
#endif

        protected abstract void OnPerformFunc();

        private void OnPerformFunc(InputAction.CallbackContext context)
        {
            if (_onlyInGame && !GameManager.InGame)
            {
                return;
            }

            if (_developmentOnly && !SymbolsHelper.IsInDevelopment())
            {
                return;
            }

            OnPerformFunc();
        }
    }
}