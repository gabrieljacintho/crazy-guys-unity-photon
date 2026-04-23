using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    [RequireComponent(typeof(Slider))]
    public abstract class SliderHandler : MonoBehaviour
    {
        private Slider _slider;

        public float CurrentValue
        {
            get => Slider.value;
            protected set => SetCurrentValue(value);
        }
        protected Slider Slider
        {
            get
            {
                if (_slider == null)
                {
                    _slider = GetComponent<Slider>();
                }

                return _slider;
            }
        }


        protected virtual void Awake()
        {
            Slider.onValueChanged.AddListener(OnValueChanged);
        }

        public void SetCurrentValue(float value)
        {
            if (value < Slider.minValue || value > Slider.maxValue)
            {
                Debug.LogWarning("The value \"" + value + "\" is not valid!");
                return;
            }

            Slider.value = value;
        }

        protected abstract void OnValueChanged(float value);
    }
}