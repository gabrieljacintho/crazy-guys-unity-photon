using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Patterns
{
    public abstract class NonSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T ActiveInstance => s_instances.Count > 0 ? s_instances[^1] : null;
        private static readonly List<T> s_instances = new List<T>();

        public static Action<T> ActiveInstanceChanged;


        protected virtual void OnEnable()
        {
            s_instances.Add(this as T);
            ActiveInstanceChanged?.Invoke(ActiveInstance);
        }

        protected virtual void OnDisable()
        {
            if (ActiveInstance == this)
            {
                s_instances.Remove(this as T);
                ActiveInstanceChanged?.Invoke(ActiveInstance);
            }
            else
            {
                s_instances.Remove(this as T);
            }
        }
    }
}