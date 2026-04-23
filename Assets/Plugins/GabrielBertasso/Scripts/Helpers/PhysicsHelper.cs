using System.Collections.Generic;
using GabrielBertasso.Extensions;
using GabrielBertasso.Physics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GabrielBertasso.Helpers
{
    public static class PhysicsHelper
    {
        public static bool HasObstacleBetween(Vector3 pointA, Vector3 pointB, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction,
            List<Collider> excludedColliders = null)
        {
            if (layerMask.IsEmpty())
            {
                return false;
            }
            
            Vector3 direction = (pointB - pointA).normalized;
            Ray ray = new Ray(pointA, direction);

            float maxDistance = Vector3.Distance(pointA, pointB) + UnityEngine.Physics.defaultContactOffset;
            
            RaycastHit[] hits = UnityEngine.Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);

            foreach (RaycastHit hit in hits)
            {
                if (excludedColliders == null || !excludedColliders.Contains(hit.collider))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasObstacleBetween(Vector3 pointA, Vector3 pointB, List<DetectionMask> masks, List<Collider> excludedColliders = null)
        {
            LayerMask layerMask = masks.GetResultLayerMask();
            if (layerMask.IsEmpty())
            {
                return false;
            }

            Vector3 direction = (pointB - pointA).normalized;
            Ray ray = new Ray(pointA, direction);

            float maxDistance = Vector3.Distance(pointA, pointB) + UnityEngine.Physics.defaultContactOffset;
            QueryTriggerInteraction triggerInteraction = masks.GetResultTriggerInteraction();

            RaycastJob(ray, out RaycastHit[] hits, maxDistance, masks.Count, layerMask, triggerInteraction);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && (excludedColliders == null || !excludedColliders.Contains(hit.collider)) && masks.CanDetect(hit.collider))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasObstacleBetween(Vector3 originPosition, Collider targetCollider, List<DetectionMask> masks)
        {
            LayerMask layerMask = masks.GetResultLayerMask();
            if (layerMask.IsEmpty())
            {
                return false;
            }

            Vector3 targetPosition = targetCollider.AccurateClosestPoint(originPosition);
            List<Collider> excludedColliders = new() { targetCollider };

            return HasObstacleBetween(originPosition, targetPosition, masks, excludedColliders);
        }

        #region Jobs

        public static void RaycastJob(Ray ray, out RaycastHit[] hits, float maxDistance, int maxHits, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            QueryParameters queryParameters = new QueryParameters(layerMask, false, queryTriggerInteraction);

            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);
            commands[0] = new RaycastCommand(ray.origin, ray.direction, queryParameters, maxDistance);
            var results = new NativeArray<RaycastHit>(maxHits, Allocator.TempJob);

            JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, maxHits, default(JobHandle));
            handle.Complete();
            commands.Dispose();

            hits = results.ToArray();
            results.Dispose();
        }

        public static void SphereCastJob(Ray ray, float radius, out RaycastHit[] hits, float maxDistance, int maxHits,
            int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            QueryParameters queryParameters = new QueryParameters(layerMask, false, queryTriggerInteraction);

            var commands = new NativeArray<SpherecastCommand>(1, Allocator.TempJob);
            commands[0] = new SpherecastCommand(ray.origin, radius, ray.direction, queryParameters, maxDistance);
            var results = new NativeArray<RaycastHit>(maxHits, Allocator.TempJob);

            JobHandle handle = SpherecastCommand.ScheduleBatch(commands, results, 1, maxHits, default(JobHandle));
            handle.Complete();
            commands.Dispose();

            hits = results.ToArray();
            results.Dispose();
        }

        #endregion
    }
}