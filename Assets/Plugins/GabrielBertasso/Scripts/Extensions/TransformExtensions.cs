using System.Collections.Generic;
using DG.Tweening;
using GabrielBertasso.DataStructures;
using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class TransformExtensions
    {
        public static TransformValues Values(this Transform transform)
        {
            return new TransformValues(transform);
        }

        public static Dictionary<Transform, TransformValues> AllValues(this Transform transform)
        {
            Dictionary<Transform, TransformValues> transformValues = new();

            transformValues.Add(transform, transform.Values());

            foreach (Transform child in transform)
            {
                Dictionary<Transform, TransformValues> childValues = AllValues(child);
                transformValues.AddRange(childValues);
            }

            return transformValues;
        }

        public static void SetValues(this Transform transform, TransformValues values, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                transform.localPosition = values.Position;
                transform.localRotation = values.Rotation;
            }
            else
            {
                transform.position = values.Position;
                transform.rotation = values.Rotation;
            }

            transform.localScale = values.Scale;
        }

        public static void LoadValues(this Dictionary<Transform, TransformValues> values, Space space = Space.Self)
        {
            foreach (KeyValuePair<Transform, TransformValues> transformValue in values)
            {
                if (transformValue.Key != null)
                    transformValue.Key.SetValues(transformValue.Value, space);
            }
        }

        public static Transform[] GetChildren(this Transform transform)
        {
            int childCount = transform.childCount;
            Transform[] children = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            return children;
        }

        public static List<Transform> GetChildrenRecursively(this Transform transform)
        {
            List<Transform> children = transform.GetChildren().ToList();

            List<Transform> otherChildren = new List<Transform>();
            foreach (Transform child in children)
            {
                otherChildren.AddRange(child.GetChildrenRecursively());
            }

            children.AddRange(otherChildren);

            return children;
        }

        public static bool ContainsChild(this Transform transform, Transform target, bool checkRecursively = false)
        {
            if (checkRecursively)
            {
                List<Transform> children = transform.GetChildrenRecursively();
                return children.Contains(target);
            }
            else
            {
                Transform[] children = transform.GetChildren();
                return children.Contains(target);
            }
        }

        public static bool CheckAngle(this Transform transform, Vector3 targetPosition, float angle)
        {
            return transform.position.CheckAngle(targetPosition, transform.forward, angle);
        }

        public static Tweener DOMoveAndRotate(this Transform transform, Vector3 targetPosition, Quaternion targetRotation, float duration)
        {
            return DOVirtual.Float(0f, 1f, duration, t =>
            {
                transform.SetPositionAndRotation(
                    Vector3.Lerp(transform.position, targetPosition, t),
                    Quaternion.Slerp(transform.rotation, targetRotation, t));
            });
        }

        public static Tweener DOMoveAndRotate(this Transform transform, Transform target, float duration)
        {
            return DOVirtual.Float(0f, 1f, duration, t =>
            {
                transform.SetPositionAndRotation(
                    Vector3.Lerp(transform.position, target.position, t),
                    Quaternion.Slerp(transform.rotation, target.rotation, t));
            });
        }

        public static Tweener DOMoveAndRotate(this Transform transform, Vector3 targetPosition, Quaternion targetRotation, float duration, AnimationCurve animationCurve)
        {
            return DOVirtual.Float(0f, 1f, duration, t =>
            {
                float x = animationCurve.Evaluate(t);
                transform.SetPositionAndRotation(
                    Vector3.Lerp(transform.position, targetPosition, x),
                    Quaternion.Slerp(transform.rotation, targetRotation, x));
            });
        }

        public static Tweener DOMoveAndRotate(this Transform transform, Transform target, float duration, AnimationCurve animationCurve)
        {
            return DOVirtual.Float(0f, 1f, duration, t =>
            {
                float x = animationCurve.Evaluate(t);
                transform.SetPositionAndRotation(
                    Vector3.Lerp(transform.position, target.position, x),
                    Quaternion.Slerp(transform.rotation, target.rotation, x));
            });
        }
    }
}