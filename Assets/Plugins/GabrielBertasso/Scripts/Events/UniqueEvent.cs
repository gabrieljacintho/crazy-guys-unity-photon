#if GABRIEL_BERTASSO_SAVE
using GabrielBertasso.Save;
#endif
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Events
{
    public class UniqueEvent :
#if GABRIEL_BERTASSO_SAVE
        SaveComponent<UniqueEvent>
#else
        MonoBehaviour
#endif
    {
        [Tooltip("Set negative to not limit.")]
        [SerializeField] private int _maxInvokeCount = 1;
        [SerializeField] private IntVariable _invokeCountVariable;
        [SerializeField] private bool _countOnInvoke = true;
        [SerializeField] private bool _invokeOnEnable;
        [SerializeField] private bool _disableObjectOnInvokeAll;

        private int _invokeCount;

        [ShowInInspector, ReadOnly]
        private int InvokeCount
        {
            get => _invokeCountVariable != null ? _invokeCountVariable.Value : _invokeCount;
            set => SetInvokeCount(value);
        }
        private bool CanInvoke => _maxInvokeCount < 0 || InvokeCount < _maxInvokeCount;

        [Space]
        public UnityEvent<int> OnInvoke;
        public UnityEvent OnInvokeAll;
        public UnityEvent OnInvokeAllLoaded;


        private void OnEnable()
        {
            if (_invokeOnEnable)
            {
                Invoke();
            }
        }

        [Button]
        public void Invoke()
        {
            if (!CanInvoke)
            {
                return;
            }

            OnInvoke?.Invoke(InvokeCount);

            if (_countOnInvoke)
            {
                Count();
            }
        }

        [Button]
        public void Count()
        {
            InvokeCount++;
        }

        public void ResetCount()
        {
            InvokeCount = 0;
        }

        private void SetInvokeCount(int value, bool loaded = false)
        {
            _invokeCount = value;

            if (_invokeCountVariable != null)
            {
                _invokeCountVariable.Value = value;
            }

            if (!CanInvoke)
            {
                if (loaded)
                {
                    OnInvokeAllLoaded?.Invoke();
                }
                else
                {
                    OnInvokeAll?.Invoke();
                }

                if (_disableObjectOnInvokeAll)
                {
                    gameObject.SetActive(false);
                }
            }
        }

#if GABRIEL_BERTASSO_SAVE
        #region Save

        protected override void SaveOperation(ES3Settings settings)
        {
            ES3.Save(SaveKey, InvokeCount, settings);
        }

        protected override void LoadOperation(ES3Settings settings)
        {
            SetInvokeCount(ES3.Load<int>(SaveKey, settings), true);
        }

        protected override void LoadDefaultValues()
        {
            SetInvokeCount(0, true);
        }

        #endregion
#endif
    }
}