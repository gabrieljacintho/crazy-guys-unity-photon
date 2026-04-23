using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.UI.Selectables
{
    [RequireComponent(typeof(Graphic))]
    public class SelectableGraphic : SelectableComponent
    {
        [Header("States")]
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _selectedSlotColor = Color.white;
        [SerializeField] private Color _disabledColor = Color.grey;

        private Graphic _graphic;

        private Color _normalColor;

        public Graphic Graphic
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                    _normalColor = _graphic.color;
                }

                return _graphic;
            }
        }
        public Color NormalColor
        {
            get => _normalColor;
            set
            {
                if (_normalColor != value)
                {
                    _normalColor = value;
                    UpdateComponent();
                }
            }
        }
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    UpdateComponent();
                }
            }
        }
        public Color SelectedSlotColor
        {
            get => _selectedSlotColor;
            set
            {
                if (_selectedSlotColor != value)
                {
                    _selectedSlotColor = value;
                    UpdateComponent();
                }
            }
        }
        public Color DisabledColor
        {
            get => _disabledColor;
            set
            {
                if (_disabledColor != value)
                {
                    _disabledColor = value;
                    UpdateComponent();
                }
            }
        }


        public override void UpdateComponent()
        {
            Graphic.color = GetCurrentStateColor();
        }

        public Color GetCurrentStateColor()
        {
            switch (CurrentState)
            {
                case SelectableState.Normal:
                    return _normalColor;

                case SelectableState.Selected:
                    return _selectedColor;

                case SelectableState.SelectedSlot:
                    return _selectedSlotColor;

                case SelectableState.Disabled:
                    return _disabledColor;

                default:
                    return default;
            }
        }
    }
}