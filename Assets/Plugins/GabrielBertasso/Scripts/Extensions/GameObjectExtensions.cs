using System.Linq;
using GabrielBertasso.Cache;
using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public static void SetActiveChildren(this GameObject gameObject, bool value)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(value);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent(out T t))
            {
                t = gameObject.AddComponent<T>();
            }

            return t;
        }

        public static Component GetOrAddComponent(this GameObject gameObject, System.Type type)
        {
            if (!gameObject.TryGetComponent(type, out Component component))
            {
                component = gameObject.AddComponent(type);
            }

            return component;
        }

        public static T[] GetInterfacesInChildren<T>(this GameObject gameObject, bool includeInactive = false)
        {
            return gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive).OfType<T>().ToArray();
        }

        public static T GetComponentCached<T>(this GameObject gameObject) where T : Component
        {
            return ComponentCacheManager.GetComponent<T>(gameObject);
        }

        public static bool TryGetComponentCached<T>(this GameObject gameObject, out T component) where T : Component
        {
            return ComponentCacheManager.TryGetComponent<T>(gameObject, out component);
        }
    }
}