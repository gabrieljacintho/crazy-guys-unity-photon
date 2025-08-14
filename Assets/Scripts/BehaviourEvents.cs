using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso
{
    public class BehaviourEvents : MonoBehaviour
    {
        public UnityEvent OnStartEvent;
        public UnityEvent OnEnableEvent;
        public UnityEvent OnDisableEvent;


        private void Start()
        {
            OnStartEvent?.Invoke();
        }

        private void OnEnable()
        {
            OnEnableEvent?.Invoke();
        }

        private void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }
    }
}