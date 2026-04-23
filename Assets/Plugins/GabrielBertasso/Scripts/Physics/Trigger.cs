using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Physics
{
    public class Trigger : DetectorBase
    {
        [Space]
        [SerializeField] private bool _canInvokeOnEnter = true;
        [ShowIf("_canInvokeOnEnter")]
        [InspectorName("On Trigger Enter")] public UnityEvent<Collider> OnTriggerEnterEvent;
        [SerializeField] private bool _canInvokeOnExit;
        [ShowIf("_canInvokeOnExit")]
        [InspectorName("On Trigger Exit")] public UnityEvent<Collider> OnTriggerExitEvent;


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_canInvokeOnEnter && CanDetect(other))
            {
                OnTriggerEnterEvent?.Invoke(other);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (_canInvokeOnExit && CanDetect(other))
            {
                OnTriggerExitEvent?.Invoke(other);
            }
        }
    }
}