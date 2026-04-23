using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Input
{
    public class ControlSchemeBehaviour : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _monoBehaviour;
        [SerializeField] private List<ControlScheme> _activeControlSchemes;


        private void OnEnable()
        {
            InputManager.ControlSchemeChanged += OnControlSchemeChanged;
            OnControlSchemeChanged(InputManager.CurrentControlScheme);
        }

        private void OnDisable()
        {
            InputManager.ControlSchemeChanged -= OnControlSchemeChanged;
        }

        private void OnControlSchemeChanged(ControlScheme controlScheme)
        {
            if (_monoBehaviour != null)
            {
                _monoBehaviour.enabled = _activeControlSchemes != null && _activeControlSchemes.Contains(controlScheme);
            }
        }
    }
}