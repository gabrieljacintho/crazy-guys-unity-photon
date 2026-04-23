using GabrielBertasso.Extensions;
#if GABRIEL_BERTASSO_CROSS_PLATFORM
using GabrielBertasso.CrossPlatform;
#endif
#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GabrielBertasso.Input
{
    public class InputPanel : MonoBehaviour
    {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
        [HideIf("_usePlatformSpecific")]
#endif
        [SerializeField] private InputActionReference _inputReference;
#if GABRIEL_BERTASSO_CROSS_PLATFORM
        [ShowIf("_usePlatformSpecific")]
        [SerializeField] private PlatformValue<InputActionReference> _platformInputReference;
        [SerializeField] private bool _usePlatformSpecific;
#endif
        [SerializeField] private bool _useCurrentControlScheme = true;
        [HideIf("@_useCurrentControlScheme")]
        [SerializeField] private ControlScheme _controlScheme;
        [SerializeField] private bool _useBindingIndex;
        [HideIf("@!_useBindingIndex")]
        [SerializeField] private int _bindingIndex;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _inputNameText;
        [ShowIf("@_inputNameText != null")]
        [SerializeField] private bool _ignoreInputName;
        [SerializeField] private Image _bindingImage;
        [SerializeField] private TextMeshProUGUI _bindingDisplayText;

        private string _actionID;

        public TextMeshProUGUI InputNameText => _inputNameText;
        public string ActionID
        {
            get => _actionID;
            set => _actionID = value;
        }
        private InputAction InputAction
        {
            get
            {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
                if (_usePlatformSpecific)
                {
                    return _platformInputReference.Value != null ? _platformInputReference.Value.action : null;
                }
                else
#endif
                {
                    return _inputReference != null ? _inputReference.action : null;
                }
            }
        }


        private void OnEnable()
        {
            BindingsManager.BindingsUpdated += UpdateInputIcon;

            if (_inputNameText != null && !_ignoreInputName)
            {
#if I2_LOCALIZATION
                LocalizationManager.OnLocalizeEvent += UpdateInputName;
#endif
                UpdateInputName();
            }

            if (_useCurrentControlScheme)
            {
                InputManager.ControlSchemeChanged += UpdateInputIcon;
            }

            UpdateInputIcon();
        }

        private void OnDisable()
        {
            BindingsManager.BindingsUpdated -= UpdateInputIcon;

#if I2_LOCALIZATION
            if (_inputNameText != null && !_ignoreInputName)
            {
                LocalizationManager.OnLocalizeEvent -= UpdateInputName;
            }
#endif

            if (_useCurrentControlScheme)
            {
                InputManager.ControlSchemeChanged -= UpdateInputIcon;
            }
        }

        public void SetInput(InputActionReference value)
        {
            _inputReference = value;

#if GABRIEL_BERTASSO_CROSS_PLATFORM
            _usePlatformSpecific = false;
#endif

            UpdateInputName();
            UpdateInputIcon();
        }

        private void UpdateInputName()
        {
            if (_inputNameText != null && !_ignoreInputName)
            {
                InputAction inputAction = GetInputAction();
                if (inputAction != null)
                {
#if I2_LOCALIZATION
                    _inputNameText.text = GetInputAction().GetNameLocalized();
#else
                    _inputNameText.text = inputAction.name;
#endif
                }
                else
                {
                    _inputNameText.text = string.Empty;
                }

                _inputNameText.gameObject.SetActive(!string.IsNullOrEmpty(_inputNameText.text));
            }
        }

        private InputAction GetInputAction()
        {
#if GABRIEL_BERTASSO_CROSS_PLATFORM
            if (_usePlatformSpecific)
            {
                return _platformInputReference.PCValue != null ? _platformInputReference.PCValue.action : null;
            }
            else
#endif
            {
                return _inputReference != null ? _inputReference.action : null;
            }
        }

        private void UpdateInputIcon(ControlScheme controlScheme)
        {
            UpdateBindingImage(controlScheme);
            UpdateBindingDisplayName(controlScheme);
        }

        private void UpdateBindingImage(ControlScheme controlScheme)
        {
            if (_bindingImage == null)
            {
                return;
            }

            if (InputAction == null)
            {
                _bindingImage.gameObject.SetActive(false);
                return;
            }

            if (_useBindingIndex)
            {
                _bindingImage.sprite = InputAction.GetIcon(_bindingIndex, controlScheme);
            }
            else
            {
                _bindingImage.sprite = InputAction.GetIcon(controlScheme);
            }

            _bindingImage.gameObject.SetActive(_bindingImage.sprite != null);
        }

        private void UpdateBindingDisplayName(ControlScheme controlScheme)
        {
            if (_bindingDisplayText == null)
            {
                return;
            }

            if (InputAction == null)
            {
                _bindingDisplayText.gameObject.SetActive(false);
                return;
            }

            if (_bindingImage == null || !_bindingImage.gameObject.activeSelf)
            {
                if (_useBindingIndex)
                {
                    _bindingDisplayText.text = InputAction.GetDisplayName(_bindingIndex, controlScheme);
                }
                else
                {
                    _bindingDisplayText.text = InputAction.GetDisplayName(controlScheme);
                }
            }
            else
            {
                _bindingDisplayText.text = null;
            }

            _bindingDisplayText.gameObject.SetActive(!string.IsNullOrEmpty(_bindingDisplayText.text));
        }

        private void UpdateInputIcon()
        {
            if (_useCurrentControlScheme)
            {
                UpdateInputIcon(InputManager.CurrentControlScheme);
            }
            else
            {
                UpdateInputIcon(_controlScheme);
            }
        }
    }
}