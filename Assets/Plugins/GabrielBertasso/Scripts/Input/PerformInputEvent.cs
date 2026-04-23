using GabrielBertasso.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Input
{
    public class PerformInputEvent : InputComponent
    {
        [Space]
        public UnityEvent OnPerform;
        [SerializeField] private bool _invokeOnNextFrame;


        protected override void OnPerformFunc()
        {
            if (_invokeOnNextFrame)
            {
                this.DoOnNextFrame(OnPerform.Invoke);
            }
            else
            {
                OnPerform?.Invoke();
            }
        }
    }
}
