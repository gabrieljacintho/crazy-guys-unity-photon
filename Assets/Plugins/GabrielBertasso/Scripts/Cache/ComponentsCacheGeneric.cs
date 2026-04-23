using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Cache
{
    public class ComponentsCacheGeneric<T>
    {
        public readonly Dictionary<GameObject, T[]> ComponentsByObject = new Dictionary<GameObject, T[]>();


        public bool TryGetComponents(GameObject gameObject, out T[] components)
        {
            if (ComponentsByObject.TryGetValue(gameObject, out components))
            {
                return components.Length > 0;
            }

            components = gameObject.GetComponents<T>();
            ComponentsByObject.TryAdd(gameObject, components);

            return components.Length > 0;
        }

        public void Clear()
        {
            ComponentsByObject.Clear();
        }
    }
}