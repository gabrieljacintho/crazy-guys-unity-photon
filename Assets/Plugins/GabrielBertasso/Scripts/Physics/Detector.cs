using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Physics
{
    public abstract class Detector : DetectorBase
    {
        public bool IsDetecting => _detectedColliders.Count > 0;

        [ShowInInspector, ReadOnly] protected List<Collider> _detectedColliders = new List<Collider>();

        public List<Collider> DetectedColliders => _detectedColliders;

        [Space]
        [SerializeField] private bool _canInvokeEvents = true;
        [ShowIf("_canInvokeEvents")]
        public UnityEvent<bool> OnChangeDetectionState;
        [ShowIf("_canInvokeEvents")]
        public UnityEvent OnDetect;
        [ShowIf("_canInvokeEvents")]
        public UnityEvent OnNotDetect;
        [ShowIf("_canInvokeEvents")]
        [SerializeField] protected bool _canInvokeOnDisable = true;
        [ShowIf("_canInvokeEvents")]
        [SerializeField] protected bool _alwaysInvoke;


        protected virtual void OnDisable()
        {
            if (IsDetecting)
            {
                _detectedColliders.Clear();

                if (_canInvokeEvents && _canInvokeOnDisable)
                {
                    OnNotDetect?.Invoke();
                }
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = IsDetecting ? Color.red : Color.white;

            foreach (Collider other in _detectedColliders)
            {
                Gizmos.DrawLine(transform.position, other.transform.position);
            }
        }

        public List<Collider> GetCollidersOrderByDistance()
        {
            _detectedColliders.RemoveAll(collider => collider == null);
            return _detectedColliders.OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position)).ToList();
        }

        protected void TryInvokeEvents(bool wasDetecting)
        {
            if (!_canInvokeEvents)
            {
                return;
            }

            if (IsDetecting)
            {
                if (!wasDetecting || _alwaysInvoke)
                {
                    OnChangeDetectionState?.Invoke(true);
                    OnDetect?.Invoke();
                }
            }
            else if (wasDetecting || _alwaysInvoke)
            {
                OnChangeDetectionState?.Invoke(false);
                OnNotDetect?.Invoke();
            }
        }
    }
}