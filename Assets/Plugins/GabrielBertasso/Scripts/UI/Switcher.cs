using GabrielBertasso.Helpers;
#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class Switcher : MonoBehaviour
    {
        [SerializeField, MinValue(0)] protected int _currentIndex;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private bool _wrap = true;

        private TextMeshProUGUI _text;

        protected TextMeshProUGUI Text
        {
            get
            {
                if (_text == null)
                {
                    _text = GetComponent<TextMeshProUGUI>();
                }

                return _text;
            }
        }
        protected abstract int ValuesLength { get; }
        public int CurrentIndex
        {
            get => _currentIndex;
            protected set => SetCurrentIndex(value);
        }

        [Space]
        [FormerlySerializedAs("onValueChanged")]
        public UnityEvent<int> OnValueChanged;


        protected virtual void Awake()
        {
            if (_previousButton != null)
            {
                _previousButton.onClick.AddListener(PreviousValue);
            }

            if (_nextButton != null)
            {
                _nextButton.onClick.AddListener(NextValue);
            }
        }

        protected virtual void OnEnable()
        {
#if I2_LOCALIZATION
            LocalizationManager.OnLocalizeEvent += UpdateText;
#endif
            UpdateText();
            UpdateButtons();
        }

        protected virtual void OnDisable()
        {
#if I2_LOCALIZATION
            LocalizationManager.OnLocalizeEvent -= UpdateText;
#endif
        }

        [Button]
        public void PreviousValue()
        {
            int previousIndex = CurrentIndex - 1;
            if (_wrap)
            {
                previousIndex = MathHelper.WrapIndex(ValuesLength, previousIndex);
            }

            if (IsValidIndex(previousIndex))
            {
                SetCurrentIndex(previousIndex);
            }
        }

        [Button]
        public void NextValue()
        {
            int nextIndex = CurrentIndex + 1;
            if (_wrap)
            {
                nextIndex = MathHelper.WrapIndex(ValuesLength, nextIndex);
            }

            if (IsValidIndex(nextIndex))
            {
                SetCurrentIndex(nextIndex);
            }
        }

        public void SetCurrentIndex(int value)
        {
            if (_wrap)
            {
                value = MathHelper.WrapIndex(ValuesLength, value);
            }

            if (!IsValidIndex(value))
            {
                Debug.LogError("The index \"" + value + "\" is not valid!", this);
                return;
            }

            if (_currentIndex == value)
            {
                return;
            }

            _currentIndex = value;

            UpdateText();
            UpdateButtons();

            OnValueChanged?.Invoke(_currentIndex);
        }

        protected virtual void UpdateText()
        {
            Text.text = GetText(CurrentIndex);
        }

        private void UpdateButtons()
        {
            if (_previousButton != null)
            {
                int previousIndex = CurrentIndex - 1;
                _previousButton.interactable = IsValidIndex(previousIndex) || _wrap;
            }

            if (_nextButton != null)
            {
                int nextIndex = CurrentIndex + 1;
                _nextButton.interactable = IsValidIndex(nextIndex) || _wrap;
            }
        }

        protected abstract string GetText(int index);

        protected bool IsValidIndex(int index) => index >= 0 && index < ValuesLength;
    }
}