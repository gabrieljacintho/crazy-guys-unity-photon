using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.AtomsAddon
{
    public class ConditionalEventTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("When true, Trigger() invokes OnTrue; otherwise invokes OnFalse.")]
        private BoolReference _condition = new BoolReference(false);

        public bool Condition
        {
            get => _condition.Value;
            set => _condition.Value = value;
        }

        [Space]
        public UnityEvent OnTrue;
        public UnityEvent OnFalse;


        public void Trigger()
        {
            if (_condition.Value)
            {
                OnTrue?.Invoke();
            }
            else
            {
                OnFalse?.Invoke();
            }
        }
    }
}
