using TMPro;
using UnityEngine;

namespace GabrielBertasso.UI
{
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;


        private void Awake()
        {
            _text.text = $"v{Application.version}";
        }
    }
}