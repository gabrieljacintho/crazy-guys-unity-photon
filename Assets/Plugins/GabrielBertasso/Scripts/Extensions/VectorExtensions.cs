using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class VectorExtensions
    {
        public static bool CheckAngle(this Vector3 originPosition, Vector3 targetPosition, Vector3 forward, float angle)
        {
            if (angle <= 0f)
            {
                return false;
            }

            if (angle >= 360f)
            {
                return true;
            }

            Vector3 direction = (targetPosition - originPosition).normalized;

            if (Vector3.Angle(forward, direction) > angle / 2f)
            {
                return false;
            }

            return true;
        }
    }
}
