using UnityEngine;

namespace GabrielBertasso.Helpers
{
    public static class GizmosHelper
    {
        public static void DrawAngle(Vector3 position, Vector3 eulerAngles, float angle, float distance = 2f)
        {
            Vector3 viewAngleL = DirectionFromAngle(eulerAngles.y, -angle / 2);
            Vector3 viewAngleR = DirectionFromAngle(eulerAngles.y, angle / 2);

            Gizmos.DrawLine(position, position + viewAngleL * distance);
            Gizmos.DrawLine(position, position + viewAngleR * distance);
        }

        public static void DrawAngle(Transform transform, float angle, float distance = 2f)
        {
            DrawAngle(transform.position, transform.eulerAngles, angle, distance);
        }

        private static Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;
            float x = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            float z = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);

            return new Vector3(x, 0, z);
        }
    }
}