using GabrielBertasso.Extensions;
using GabrielBertasso.Helpers;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    [RequireComponent(typeof(Collider))]
    public class Sensor : Detector
    {
        [Header("Sensor")]
        [SerializeField, Range(0f, 360f)] private float _angle = 360f;
        [SerializeField, Min(1)] private int _updateInterval = 1;

        private bool CanUpdate => Time.frameCount % _updateInterval == 0;


        private void FixedUpdate()
        {
            int n = _detectedColliders.RemoveAll(x => x == null || !x.enabled || !x.gameObject.activeInHierarchy);
            if (n > 0)
            {
                TryInvokeEvents(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActiveAndEnabled || !CanDetect(other) || _detectedColliders.Contains(other))
            {
                return;
            }

            bool wasDetecting = IsDetecting;

            _detectedColliders.Add(other);

            TryInvokeEvents(wasDetecting);
        }

        /*private void OnTriggerStay(Collider other)
        {
            if (!isActiveAndEnabled || !CanUpdate)
            {
                return;
            }
            
            bool wasDetecting = IsDetecting;

            if (CanDetect(other))
            {
                if (!_detectedColliders.Contains(other))
                {
                    _detectedColliders.Add(other);
                }
            }
            else
            {
                _detectedColliders.Remove(other);
            }

            TryInvokeEvents(wasDetecting);
        }*/

        private void OnTriggerExit(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            bool wasDetecting = IsDetecting;

            _detectedColliders.Remove(other);

            TryInvokeEvents(wasDetecting);
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (_angle <= 0 || _angle >= 360f)
            {
                return;
            }

            Gizmos.color = Color.white;
            GizmosHelper.DrawAngle(transform, _angle);
        }

        public override bool CanDetect(Collider targetCollider)
        {
            return transform.CheckAngle(targetCollider.transform.position, _angle) && base.CanDetect(targetCollider);
        }
    }
}