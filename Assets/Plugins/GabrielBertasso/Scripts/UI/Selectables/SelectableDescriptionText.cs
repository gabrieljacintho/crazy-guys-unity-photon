using GabrielBertasso.Extensions;
using TMPro;
using UnityEngine;

namespace GabrielBertasso.UI.Selectables
{
    public class SelectableDescriptionText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;


        private void OnEnable()
        {
            EventManager.ActualSelectedObjectChanged += MainSelectedObjectChanged;
            MainSelectedObjectChanged(EventManager.ActualSelectedObject);
        }

        private void OnDisable()
        {
            EventManager.ActualSelectedObjectChanged -= MainSelectedObjectChanged;
        }

        private void MainSelectedObjectChanged(GameObject selectedObject)
        {
            SelectableDescription description = selectedObject.GetComponentCached<SelectableDescription>();
            _text.text = description != null ? description.Description : string.Empty;
        }
    }
}