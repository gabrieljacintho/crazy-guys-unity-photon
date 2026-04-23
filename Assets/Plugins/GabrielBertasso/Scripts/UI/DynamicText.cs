using System.Collections.Generic;
using GabrielBertasso.Extensions;
#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace GabrielBertasso.UI
{
    public class DynamicText : MonoBehaviour
    {
        [SerializeField] private bool _useTextWriter;
        [ShowIf("_useTextWriter")]
        [SerializeField] private TextWriter _textWriter;
        [HideIf("_useTextWriter")]
        [SerializeField] private TextMeshProUGUI _text;
#if I2_LOCALIZATION
        [SerializeField] private bool _useLocalizedText;
        [ShowIf("_useLocalizedText")]
        [SerializeField] private LocalizedString _targetLocalizedText;
        [HideIf("_useLocalizedText")]
#endif
        [SerializeField, TextArea(10, 30)] private string _targetText;
        [SerializeField] private List<TagValue> _tagValues;


        private void OnEnable()
        {
#if I2_LOCALIZATION
            LocalizationManager.OnLocalizeEvent += UpdateText;

            if (_useLocalizedText)
            {
                LocalizationManager.OnLocalizeEvent += UpdateText;
            }
#endif

            UpdateText();
        }

#if I2_LOCALIZATION
        private void OnDisable()
        {
            LocalizationManager.OnLocalizeEvent -= UpdateText;

            if (_useLocalizedText)
            {
                LocalizationManager.OnLocalizeEvent -= UpdateText;
            }
        }
#endif

        [Button]
        private void UpdateText()
        {
#if I2_LOCALIZATION
            string newText = _useLocalizedText ? _targetLocalizedText : _targetText;
#else
            string newText = _targetText;
#endif

            foreach (TagValue value in _tagValues)
            {
                newText = newText.Replace(value.Tag, value.GetText());
            }

            newText = newText.FixText();

            if (_useTextWriter)
            {
                _textWriter.TargetText = newText;
            }
            else
            {
                _text.text = newText;
            }
        }
    }
}
