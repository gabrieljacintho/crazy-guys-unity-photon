using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class ColliderExtensions
    {
        public static bool IsInside(this Collider collider, Vector3 point)
        {
            return collider.bounds.Contains(point);
        }
        
        public static Vector3 NearestVertexTo(this MeshCollider meshCollider, Vector3 position)
        {
            position = meshCollider.transform.InverseTransformPoint(position);

            float minDistanceSqr = Mathf.Infinity;
            Vector3 nearestVertex = Vector3.zero;

            List<Vector3> vertices = new List<Vector3>();
            meshCollider.sharedMesh.GetVertices(vertices);
            foreach (Vector3 vertex in vertices)
            {
                Vector3 diff = position - vertex;
                float distanceSqr = diff.sqrMagnitude;
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearestVertex = vertex;
                }
            }
            
            return meshCollider.transform.TransformPoint(nearestVertex);
        }

        public static Vector3 AccurateClosestPoint(this Collider collider, Vector3 position)
        {
            if (collider is MeshCollider meshCollider && !meshCollider.convex)
            {
                return meshCollider.NearestVertexTo(position);
            }
            
            return collider.ClosestPoint(position);
        }
    }
}