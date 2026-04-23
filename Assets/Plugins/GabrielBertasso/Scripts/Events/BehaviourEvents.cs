using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Events
{
    public class BehaviourEvents : MonoBehaviour
    {
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onDisable;


        private void Start()
        {
            onStart?.Invoke();
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }
    }
}
