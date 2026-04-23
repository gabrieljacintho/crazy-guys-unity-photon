#if I2_LOCALIZATION
using I2.Loc;
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GabrielBertasso.UI.Selectables
{
    public class SelectableDescription : MonoBehaviour
    {
#if I2_LOCALIZATION
        [SerializeField] private bool _useLocalizedText;
        [ShowIf("_useLocalizedText")]
        [SerializeField] private LocalizedString _localizedDescription;
        [HideIf("_useLocalizedText")]
#endif
        [SerializeField] private string _description;

        public string Description
        {
            get
            {
#if I2_LOCALIZATION
                return _useLocalizedText ? _localizedDescription : _description;
#else
                return _description;
#endif
            }
        }
    }
}