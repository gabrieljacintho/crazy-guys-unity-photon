using System;
using GabrielBertasso.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GabrielBertasso.UI
{
    public class SelectGameObject : MonoBehaviour
    {
        [Serializable]
        public enum SelectMode
        {
            GameObject,
            Child,
            FirstSelectableInChildren
        }

        [SerializeField] private SelectMode _mode;
        [ShowIf("@_mode == SelectMode.GameObject")]
        [FormerlySerializedAs("targetObject")]
        [SerializeField] private GameObject _targetObject;
        [ShowIf("@_mode == SelectMode.Child")]
        [SerializeField] private int _childIndex;
        [FormerlySerializedAs("selectOnEnable")]
        [SerializeField] private bool _selectOnEnable;

        [Space]
        [FormerlySerializedAs("onSelect")]
        public UnityEvent OnSelect;


        private void OnEnable()
        {
            if (_selectOnEnable)
            {
                this.DoOnNextFrame(Select);
            }
        }

        [Button]
        public void Select()
        {
            if (EventManager.EventSystem == null)
            {
                Debug.LogError("EventSystem not found!", this);
                return;
            }

            GameObject targetObject = GetTargetObject();
            EventManager.EventSystem.SetSelectedGameObject(targetObject);

            OnSelect?.Invoke();
        }

        private GameObject GetTargetObject()
        {
            switch (_mode)
            {
                case SelectMode.GameObject:
                    return _targetObject;

                case SelectMode.Child:
                    return transform.childCount > _childIndex ? transform.GetChild(_childIndex).gameObject : null;

                case SelectMode.FirstSelectableInChildren:
                    var result = gameObject.GetComponentInChildren<Selectable>();
                    return result != null ? result.gameObject : null;

                default:
                    return null;
            }
        }
    }
}