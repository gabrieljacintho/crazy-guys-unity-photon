using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GabrielBertasso.Helpers
{
    public static class ResourcesHelper
    {
        public static T[] LoadAll<T>(string path = "") where T : Object
        {
            path ??= "";
            Type type = typeof(T);

            Object[] objects = Resources.LoadAll(path, type);
            if (objects == null || objects.Length == 0)
            {
                Debug.LogWarning("There is no " + type.Name + " in path \"" + path + "\"!");
                return Array.Empty<T>();
            }

            T[] items = new T[objects.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = (T)objects[i];
            }

            return items;
        }
    }
}