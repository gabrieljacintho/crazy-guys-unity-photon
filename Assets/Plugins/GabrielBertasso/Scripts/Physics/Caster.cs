using System.Collections.Generic;
using GabrielBertasso.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    public abstract class Caster : Detector
    {
        [SerializeField] private bool _useFixedUpdate = true;
        [SerializeField, Min(1)] protected int _maxAmountOfHits = 8;

        [Space]
        [SerializeField, Min(1)] protected int _updateInterval = 1;

        public List<RaycastHit> Hits { get; private set; } = new List<RaycastHit>();

        private bool CanUpdate => (_useFixedUpdate ? UpdateManager.FixedUpdateCount : Time.frameCount) % _updateInterval == 0;


        private void Update()
        {
            if (_useFixedUpdate || !CanUpdate)
            {
                return;
            }

            UpdateDetector();
        }

        private void FixedUpdate()
        {
            if (!_useFixedUpdate || !CanUpdate)
            {
                return;
            }

            UpdateDetector();
        }

        [Button("Update")]
        public virtual void UpdateDetector()
        {
            bool wasDetecting = IsDetecting;

            Hits = GetDetectedHits();
            _detectedColliders.Clear();
            _detectedColliders.AddRange(Hits.Select(hit => hit.collider));

            TryInvokeEvents(wasDetecting);
        }

        public bool TryGetClosestHit(out RaycastHit hit)
        {
            hit = default;
            if (Hits == null || Hits.Count == 0)
            {
                return false;
            }

            hit = Hits[0];
            if (Hits.Count == 1)
            {
                return true;
            }

            foreach (RaycastHit other in Hits)
            {
                if (other.distance < hit.distance)
                {
                    hit = other;
                }
            }

            return true;
        }

        private List<RaycastHit> GetDetectedHits()
        {
            Cast(out RaycastHit[] results, _targetLayerMask);

            List<RaycastHit> hits = new List<RaycastHit>();
            for (int i = 0; i < results.Length; i++)
            {
                RaycastHit hit = results[i];
                if (hit.collider != null && CanDetect(hit.collider, hit.point))
                {
                    hits.Add(hit);
                }
            }

            return hits;
        }

        protected abstract void Cast(out RaycastHit[] results, int layerMask = ~0);
    }
}