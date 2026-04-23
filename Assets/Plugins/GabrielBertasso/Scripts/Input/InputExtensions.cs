#if I2_LOCALIZATION
using GabrielBertasso.I2LocalizationAddon;
#endif
using GabrielBertasso.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace GabrielBertasso.Extensions
{
    public static class InputExtensions
    {
        #region Icon

        public static Sprite GetIcon(this InputAction input, ControlScheme controlScheme)
        {
            return BindingsManager.Instance != null ? BindingsManager.Instance.GetIcon(input, controlScheme) : null;
        }

        public static Sprite GetIcon(this InputAction input, int bindingIndex, ControlScheme controlScheme)
        {
            return BindingsManager.Instance != null ? BindingsManager.Instance.GetIcon(input, bindingIndex, controlScheme) : null;
        }

        #endregion

        #region DisplayName

        public static string GetDisplayName(this InputAction input, ControlScheme controlScheme,
            InputBinding.DisplayStringOptions option = InputBinding.DisplayStringOptions.DontIncludeInteractions)
        {
            if (FindFirstBindingWithControlScheme(input, controlScheme, out InputBinding binding))
            {
                return binding.ToDisplayString(option);
            }

            return string.Empty;
        }

        public static string GetDisplayName(this InputAction input, int bindingIndex, ControlScheme controlScheme,
            InputBinding.DisplayStringOptions option = InputBinding.DisplayStringOptions.DontIncludeInteractions)
        {
            ReadOnlyArray<InputBinding> bindings = input.bindings;

            if (bindings.Count <= bindingIndex)
            {
                return string.Empty;
            }

            return bindings[bindingIndex].ToDisplayString(option);
        }

        #endregion

#if I2_LOCALIZATION
        #region NameLocalized

        public static string GetNameLocalized(this InputAction input)
        {
            return input.name.GetLocalized("Controls");
        }

        #endregion
#endif

        public static bool FindFirstBindingWithControlScheme(this InputAction input, ControlScheme controlScheme, out InputBinding binding)
        {
            ReadOnlyArray<InputBinding> bindings = input.bindings;

            foreach (InputBinding otherBinding in bindings)
            {
                string groups = otherBinding.groups;

                switch (controlScheme)
                {
                    case ControlScheme.KeyboardMouse:
                        if (groups.Contains("Keyboard Mouse"))
                        {
                            binding = otherBinding;
                            return true;
                        }
                        break;

                    case ControlScheme.XboxController:
                        if (groups.Contains("Xbox Controller"))
                        {
                            binding = otherBinding;
                            return true;
                        }
                        break;

                    case ControlScheme.PlayStationController:
                        if (groups.Contains("PlayStation Controller"))
                        {
                            binding = otherBinding;
                            return true;
                        }
                        break;
                }
            }

            foreach (InputBinding otherBinding in bindings)
            {
                string groups = otherBinding.groups;

                if (string.IsNullOrEmpty(groups))
                {
                    binding = otherBinding;
                    return true;
                }
            }

            binding = default;
            return false;
        }
    }
}