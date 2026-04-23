using System;
using GabrielBertasso.Extensions;
#if FMOD
using GabrielBertasso.FMODIntegration;
#endif
#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextWriter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
#if I2_LOCALIZATION
        [SerializeField] private bool _useLocalizedText;
        [ShowIf("_useLocalizedText")]
        [SerializeField] private LocalizedString _localizedTargetText;
        [HideIf("_useLocalizedText")]
#endif
        [SerializeField] private string _targetText;
        [SerializeField, Range(0f, 1f)] private float _fillAmount;
        [SerializeField] private float _charsPerMinute = 280f;
        [SerializeField] private bool _resetOnDisable = true;
#if FMOD
        [SerializeField] private FMODEventSettings _typeAudio = new FMODEventSettings(default);
#else
        [SerializeField] private AudioClip _typeAudio;
#endif

        [Space]
        public UnityEvent OnEnded;

        private bool _isRunning;

        public string TargetText
        {
            get
            {
#if I2_LOCALIZATION
                return _useLocalizedText ? _localizedTargetText : _targetText;
#else
                return _targetText;
#endif
            }
            set
            {
#if I2_LOCALIZATION
                _useLocalizedText = false;
#endif
                _targetText = value;
                UpdateText();
            }
        }
#if I2_LOCALIZATION
        public LocalizedString LocalizedTargetText
        {
            get => _localizedTargetText;
            set
            {
                _useLocalizedText = true;
                _localizedTargetText = value;
                UpdateText();
            }
        }
#endif
        public float FillAmount
        {
            get => _fillAmount;
            set => SetFillAmount(value);
        }

        private float FillAmountPerChar => 1f / TargetText.ToString().Length;


        private void OnEnable()
        {
            _isRunning = true;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateText();
                return;
            }

            if (_charsPerMinute != 0f)
            {
                float charCount = Time.deltaTime / 60f * _charsPerMinute;
                FillAmount += charCount * FillAmountPerChar;
            }
        }

        private void LateUpdate()
        {
            if (!_isRunning || !Application.isPlaying)
            {
                return;
            }

            UpdateText();
        }

        private void OnDisable()
        {
            if (_resetOnDisable)
            {
                ResetText();
            }
        }

        public void SetFillAmount(float value)
        {
            _fillAmount = Mathf.Clamp(value, 0f, 1f);
            UpdateText();
        }

        public void ResetText()
        {
            _fillAmount = 0f;
            _text.text = string.Empty;
        }

        private void UpdateText()
        {
            int lastLength = _text.text.Length;
            string targetText = TargetText;
            _text.text = targetText.Substring(0, Mathf.CeilToInt(targetText.Length * _fillAmount));

            if (_text.text.Length != lastLength)
            {
                PlayTypeSFX();
            }

            if (_isRunning && _text.text == targetText)
            {
                _isRunning = false;
                OnEnded?.Invoke();
            }
        }

        private void PlayTypeSFX()
        {
#if FMOD
            if (_typeAudio.IsValid)
            {
                _typeAudio.Play();
            }
#else
            if (_typeAudio != null)
            {
                AudioSource.PlayClipAtPoint(_typeAudio, default);
            }
#endif
        }
    }
}