using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GabrielBertasso.Cache
{
    public static class ComponentCacheManager
    {
        private static readonly List<ComponentCache> s_componentCaches = new List<ComponentCache>();


        public static T GetComponent<T>(GameObject gameObject)
        {
            ComponentCache componentCache = GetComponentCacheOfType<T>();
            return componentCache.GetComponent<T>(gameObject);
        }

        public static bool TryGetComponent<T>(GameObject gameObject, out T component)
        {
            ComponentCache componentCache = GetComponentCacheOfType<T>();
            return componentCache.TryGetComponent<T>(gameObject, out component);
        }

        public static T GetOrAddComponent<T>(GameObject gameObject)
        {
            ComponentCache componentCache = GetComponentCacheOfType<T>();
            return componentCache.GetOrAddComponent<T>(gameObject);
        }

        private static ComponentCache GetComponentCacheOfType<T>()
        {
            Type type = typeof(T);
            ComponentCache componentCache = s_componentCaches.Find(cache => cache.Type == type);
            if (componentCache == null)
            {
                componentCache = new ComponentCache(type);
                s_componentCaches.Add(componentCache);
            }

            return componentCache;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            s_componentCaches.Clear();
        }
    }
}