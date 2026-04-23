using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Cache
{
    public class ComponentCacheGeneric<T>
    {
        public readonly Dictionary<GameObject, T> ComponentByObject = new Dictionary<GameObject, T>();


        public bool TryGetComponent(GameObject gameObject, out T component)
        {
            if (ComponentByObject.TryGetValue(gameObject, out component))
            {
                return component != null;
            }

            component = gameObject.GetComponent<T>();
            ComponentByObject.TryAdd(gameObject, component);

            return component != null;
        }

        public void Clear()
        {
            ComponentByObject.Clear();
        }
    }
}