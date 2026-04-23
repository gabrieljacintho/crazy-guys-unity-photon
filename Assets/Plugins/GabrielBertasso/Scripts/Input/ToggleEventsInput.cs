using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Input
{
    public class ToggleEventsInput : InputComponent
    {
        [Header("Toggle")]
        [SerializeField] private BoolReference _on = new BoolReference(true);
        [SerializeField] private bool _invokeEventOnStart = true;

        public bool On
        {
            get => _on.Value;
            set => _on.Value = value;
        }

        [Space]
        public UnityEvent OnTurnOn;
        public UnityEvent OnTurnOff;


        protected override void Start()
        {
            base.Start();

            if (_invokeEventOnStart)
            {
                InvokeEvent();
            }
        }

        protected override void OnPerformFunc()
        {
            SetOn(!On);
        }

        public void InvokeEvent()
        {
            if (On)
            {
                OnTurnOn?.Invoke();
            }
            else
            {
                OnTurnOff?.Invoke();
            }
        }

        public void SetOn(bool value)
        {
            if (_on.Value == value)
            {
                return;
            }

            _on.Value = value;
            InvokeEvent();
        }
    }
}