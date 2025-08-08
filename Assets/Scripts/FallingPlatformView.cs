using Quantum;
using UnityEngine;

namespace GabrielBertasso
{
    public class FallingPlatformView : QuantumEntityViewComponent
    {
        [SerializeField] private AudioClip _fallAudioClip;


        public override void OnActivate(Frame frame)
        {
            QuantumEvent.Subscribe<EventPlatformFell>(this, OnPlatformFell);
        }

        private void OnPlatformFell(EventPlatformFell callback)
        {
            if (callback.Entity != EntityRef)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(_fallAudioClip, transform.position, 1f);
        }
    }
}