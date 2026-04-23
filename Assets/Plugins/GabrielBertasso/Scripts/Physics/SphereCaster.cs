using GabrielBertasso.Helpers;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    public class SphereCaster : Caster
    {
        [Header("Sphere")]
        [SerializeField, Min(0f)] private float _radius = 1f;
        [SerializeField, Min(0f)] private float _maxDistance = 1f;


        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Vector3 offset = transform.forward * _maxDistance;
            Vector3 center = transform.position + offset;

            Gizmos.color = IsDetecting ? Color.red : Color.white;
            Gizmos.DrawWireSphere(center, _radius);
        }

        protected override void Cast(out RaycastHit[] results, int layerMask = ~0)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            PhysicsHelper.SphereCastJob(ray, _radius, out results, _maxDistance, _maxAmountOfHits, layerMask, _targetTriggerInteraction);
        }
    }
}