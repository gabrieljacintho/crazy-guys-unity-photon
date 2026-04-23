using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GabrielBertasso.UI
{
    [RequireComponent(typeof(Switcher))]
    public class SwitcherControl : MonoBehaviour
    {
        public InputActionReference input;
        public GameObject requiredSelection;

        [Header("SettingsSettings")]
        public bool continuous;
        [ShowIf("continuous")]
        [Min(0f)] public float speed = 1f;

        public Switcher Switcher { get; private set; }

        private float _t;


        private void Awake()
        {
            Switcher = GetComponent<Switcher>();
        }

        private void Update()
        {
            if (input == null)
            {
                return;
            }

            if (requiredSelection != null)
            {
                if (CanvasInput.SelectedObject != requiredSelection)
                {
                    return;
                }
            }

            Vector2 direction = input.action.ReadValue<Vector2>();

            if (continuous)
            {
                if (_t < 1f && speed > 0f)
                {
                    _t += Mathf.Abs(direction.x) * speed * Time.unscaledDeltaTime;
                }
            }
            else if (direction.x == 0)
            {
                _t = 1f;
            }

            if (_t < 1f || direction.x == 0f)
            {
                return;
            }

            if (direction.x > 0)
            {
                Switcher.NextValue();
            }
            else if (direction.x < 0)
            {
                Switcher.PreviousValue();
            }

            _t = 0f;
        }
    }
}