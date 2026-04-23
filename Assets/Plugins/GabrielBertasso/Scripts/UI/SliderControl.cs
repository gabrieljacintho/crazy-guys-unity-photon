using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderControl : MonoBehaviour
    {
        public Slider Slider { get; private set; }

        public InputActionReference input;
        public GameObject requiredSelection;

        [Header("SettingsSettings")]
        [Min(0f)] public float speed = 1f;


        private void Awake()
        {
            Slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (Slider == null || input == null || speed <= 0f)
                return;

            if (requiredSelection != null)
            {
                if (CanvasInput.SelectedObject != requiredSelection)
                {
                    return;
                }
            }

            Vector2 direction = input.action.ReadValue<Vector2>();
            float value = Slider.value + direction.x * speed * Time.unscaledDeltaTime;

            Slider.value = Mathf.Clamp(value, Slider.minValue, Slider.maxValue);
        }
    }
}