using System;
using System.Collections.Generic;
using GabrielBertasso.DataStructures;
using GabrielBertasso.Physics;
using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class ListExtensions
    {
        public static T Dequeue<T>(this List<T> list)
        {
            T value = list[0];
            list.RemoveAt(0);

            return value;
        }

        public static T FirstOrDefault<T>(this List<T> list)
        {
            return list.Count > 0 ? list[0] : default;
        }

        public static bool ContainsAll<T>(this List<T> source, List<T> target)
        {
            for (int i = 0; i < target.Count; i++)
            {
                if (!source.Contains(target[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAny<T>(this List<T> source, List<T> target)
        {
            for (int i = 0; i < target.Count; i++)
            {
                if (source.Contains(target[i]))
                {
                    return true;
                }
            }

            return false;
        }

        #region Dictionary

        public static bool TryGetValue<TKey, TValue>(this List<KeyValue<TKey, TValue>> list, TKey key, out TValue value)
        {
            bool Predicate(KeyValue<TKey, TValue> keyValue)
            {
                return key != null ? keyValue.Key != null && keyValue.Key.Equals(key) : keyValue.Key == null;
            }

            if (list.Exists(Predicate))
            {
                value = list.Find(Predicate).Value;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetValues<TKey, TValue>(this List<KeyValue<TKey, TValue>> list, TKey key, out List<TValue> values)
        {
            bool Predicate(KeyValue<TKey, TValue> keyValue)
            {
                return key != null ? keyValue.Key != null && keyValue.Key.Equals(key) : keyValue.Key == null;
            }

            if (list.Exists(Predicate))
            {
                List<KeyValue<TKey, TValue>> keyValues = list.FindAll(keyValue => keyValue.Key.Equals(key));
                values = keyValues.Select(keyValue => keyValue.Value);
                return true;
            }

            values = null;
            return false;
        }

        public static List<TResult> Select<TSource, TResult>(this List<TSource> list, Func<TSource, TResult> selector)
        {
            List<TResult> results = new List<TResult>();
            foreach (TSource item in list)
            {
                TResult result = selector.Invoke(item);
                results.Add(result);
            }

            return results;
        }

        #endregion

        #region Mask

        public static bool CanDetect(this DetectionMask mask, Collider target)
        {
            if (mask.TriggerInteraction == QueryTriggerInteraction.UseGlobal)
            {
                mask.TriggerInteraction = UnityEngine.Physics.queriesHitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
            }

            if (target.isTrigger && mask.TriggerInteraction == QueryTriggerInteraction.Ignore)
            {
                return false;
            }

            return mask.CanDetect(target.gameObject);
        }

        public static bool CanDetect(this List<DetectionMask> masks, Collider target)
        {
            return masks.Exists(x => x.CanDetect(target));
        }

        public static bool CanDetect(this DetectionMask mask, GameObject target)
        {
            return mask.LayerMask.Contains(target.layer) && (mask.Tags.Count == 0 || mask.Tags.Exists(x => target.CompareTag(x)));
        }

        public static int GetResultLayerMask(this List<DetectionMask> masks)
        {
            int layerMask = 0;
            for (int i = 0; i < masks.Count; i++)
            {
                layerMask |= masks[i].LayerMask;
            }

            return layerMask;
        }

        public static QueryTriggerInteraction GetResultTriggerInteraction(this List<DetectionMask> masks)
        {
            return masks.Exists(x => x.TriggerInteraction == QueryTriggerInteraction.Collide) ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
        }

        #endregion
    }
}
