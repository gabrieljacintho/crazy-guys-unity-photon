using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.UI
{
    public class SelectedObjectChangedEvent : MonoBehaviour
    {
        public UnityEvent<GameObject> OnSelectedObjectChanged;


        private void OnEnable()
        {
            EventManager.ActualSelectedObjectChanged += MainSelectedObjectChanged;
        }

        private void OnDisable()
        {
            EventManager.ActualSelectedObjectChanged -= MainSelectedObjectChanged;
        }

        private void MainSelectedObjectChanged(GameObject selectedObject)
        {
            if (selectedObject != null)
            {
                OnSelectedObjectChanged?.Invoke(selectedObject);
            }
        }
    }
}