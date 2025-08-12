using Quantum;
using UnityEngine;

namespace GabrielBertasso
{
    public class CoinView : QuantumEntityViewComponent
    {
        [SerializeField] private GameObject _visualRoot;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private AudioClip _collectAudioClip;
        [SerializeField] private float _rotationSpeed = 90f;


        public override void OnActivate(Frame frame)
        {
            QuantumEvent.Subscribe<EventCoinCollected>(this, OnCoinCollected, (DispatchableFilter)null, false, true);

            _visualRoot.transform.Rotate(0, Random.Range(0f, 360f), 0f, Space.World);
        }

        public override void OnUpdateView()
        {
            var coin = GetPredictedQuantumComponent<Coin>();
            bool isActive = coin.IsActive(PredictedFrame);

            _visualRoot.SetActive(isActive);

            var emission = _particles.emission;
            emission.enabled = isActive;

            if (isActive)
            {
                _visualRoot.transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.World);
            }
        }

        private void OnCoinCollected(EventCoinCollected callback)
        {
            if (callback.Entity != EntityRef)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(_collectAudioClip, transform.position, 1f);
        }
    }
}