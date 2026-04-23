using GabrielBertasso.Helpers;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    public class Raycaster : Caster
    {
        [Header("Ray")]
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField, Min(0f)] private float _maxDistance = 1f;

        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            Vector3 point1 = transform.position;

            Vector3 direction = transform.TransformDirection(_direction);
            Vector3 point2 = point1 + direction * _maxDistance;      
            
            Gizmos.color = IsDetecting ? Color.red : Color.white;
            Gizmos.DrawLine(point1, point2);
        }

        protected override void Cast(out RaycastHit[] results, int layerMask = ~0)
        {
            Ray ray = new Ray(transform.position, transform.TransformDirection(_direction));
            results = new RaycastHit[_maxAmountOfHits];
            PhysicsHelper.RaycastJob(ray, out results, _maxDistance, _maxAmountOfHits, layerMask, _targetTriggerInteraction);
        }
    }
}