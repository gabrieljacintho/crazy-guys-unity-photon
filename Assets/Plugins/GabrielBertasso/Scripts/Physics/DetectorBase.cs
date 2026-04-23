using System.Collections.Generic;
using GabrielBertasso.Extensions;
using GabrielBertasso.Helpers;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    public abstract class DetectorBase : MonoBehaviour
    {
        [SerializeField] protected List<DetectionMask> _targetMasks;
        [SerializeField] protected List<DetectionMask> _obstacleMasks;

        protected int _targetLayerMask;
        protected QueryTriggerInteraction _targetTriggerInteraction;
        protected int _obstacleLayerMask;


        protected virtual void Start()
        {
            // Show enabled toggle in editor
            _targetLayerMask = _targetMasks.GetResultLayerMask();
            _targetTriggerInteraction = _targetMasks.GetResultTriggerInteraction();
            _obstacleLayerMask = _obstacleMasks.GetResultLayerMask();
        }

        public virtual bool CanDetect(Collider targetCollider)
        {
            if (!isActiveAndEnabled || !_targetMasks.CanDetect(targetCollider))
            {
                return false;
            }

            return !PhysicsHelper.HasObstacleBetween(transform.position, targetCollider, _obstacleMasks);
        }

        public virtual bool CanDetect(Collider targetCollider, Vector3 contactPoint)
        {
            if (!isActiveAndEnabled || !_targetMasks.CanDetect(targetCollider))
            {
                return false;
            }

            List<Collider> excludedColliders = new() { targetCollider };

            return !PhysicsHelper.HasObstacleBetween(transform.position, contactPoint, _obstacleMasks, excludedColliders);
        }
    }
}