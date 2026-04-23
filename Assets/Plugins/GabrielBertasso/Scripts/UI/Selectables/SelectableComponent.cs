using GabrielBertasso.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace GabrielBertasso.UI.Selectables
{
    public enum SelectableState
    {
        Normal,
        Selected,
        SelectedSlot,
        Disabled
    }

    public abstract class SelectableComponent : MonoBehaviour
    {
        [Tooltip("If null the component GameObject is used.")]
        [SerializeField] private GameObject _selectableObject;
        [SerializeField] private bool _alwaysUpdate;

        private Selectable _selectable;

        public GameObject SelectableObject => _selectableObject != null ? _selectableObject : gameObject;
        public Selectable Selectable
        {
            get
            {
                if (_selectable == null)
                {
                    _selectable = SelectableObject.GetComponentInParent<Selectable>();
                }

                return _selectable;
            }
        }
        public SelectableState CurrentState => GetCurrentState();


        protected virtual void OnEnable()
        {
            EventManager.ActualSelectedObjectChanged += UpdateComponent;
            UpdateComponent();
            this.DoOnNextFrame(UpdateComponent);
        }

        protected virtual void Update()
        {
            if (_alwaysUpdate)
            {
                UpdateComponent();
            }
        }

        protected virtual void OnDisable()
        {
            EventManager.ActualSelectedObjectChanged -= UpdateComponent;
        }

        public abstract void UpdateComponent();

        private void UpdateComponent(GameObject selectedObject) => UpdateComponent();

        private SelectableState GetCurrentState()
        {
            if (Selectable == null || !Selectable.interactable)
            {
                return SelectableState.Disabled;
            }

            if (EventManager.ActualSelectedObject == Selectable.gameObject)
            {
                return SelectableState.Selected;
            }

            return SelectableState.Normal;
        }
    }
}