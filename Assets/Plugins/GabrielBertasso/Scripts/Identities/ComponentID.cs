using System;
using System.Collections.Generic;
using System.Linq;
using GabrielBertasso.Extensions;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    public class ComponentID : MonoBehaviour
    {
        [SerializeField] private Component _component;
        [SerializeField] private string _id;

        public static Dictionary<string, List<Component>> ComponentsByID => ComponentIDManager.Instance != null ?
            ComponentIDManager.Instance.ComponentsByID : new Dictionary<string, List<Component>>();


        private void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        public static T FindComponentWithID<T>(string id, bool includeInactive = false) where T : Component
        {
            List<T> components = FindComponentsWithID<T>(id);

            if (components.Count == 0 && includeInactive)
            {
                components = FindComponentsWithID<T>(id, true);
            }

            return components.Count > 0 ? components[0] : null;
        }

        public static List<T> FindComponentsWithID<T>(string id, bool includeInactive = false) where T : Component
        {
            if (includeInactive)
            {
                RegisterAllComponents();
            }

            if (!ComponentsByID.TryGetValue(id, out List<Component> components))
            {
                return new List<T>();
            }

            if (includeInactive)
            {
                return components.OfType<T>().ToList();
            }

            return components.FindAll(x => x.gameObject.activeInHierarchy).OfType<T>().ToList();
        }

        private void Register()
        {
            if (_component == null)
            {
                Debug.LogWarning("Component is null!", this);
                return;
            }

            if (ComponentsByID.TryGetValue(_id, out List<Component> components))
            {
                if (components.Contains(_component))
                {
                    return;
                }

                components.Add(_component);
            }
            else
            {
                ComponentsByID.Add(_id, new List<Component>() { _component });
            }
        }

        private void Unregister()
        {
            if (_component == null)
            {
                return;
            }

            if (!ComponentsByID.TryGetValue(_id, out List<Component> components) || !components.Contains(_component))
            {
                return;
            }

            components.Remove(_component);
        }

        private static void RegisterAllComponents()
        {
            ComponentID[] componentIDs = FindObjectsByType<ComponentID>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Array.ForEach(componentIDs, componentID => componentID.Register());
        }
    }
}