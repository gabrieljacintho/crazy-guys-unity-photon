using System;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    [Serializable]
    public class ComponentReference<T> where T : Component
    {
        public T Component;
        public string Id;
        public bool IncludeInactive;

        public T Value
        {
            get
            {
                if (Component != null)
                {
                    return Component;
                }

                Component = ComponentID.FindComponentWithID<T>(Id, IncludeInactive);

                return Component;
            }
            set => Component = value;
        }


        public ComponentReference(string id, bool includeInactive = false)
        {
            Id = id;
            IncludeInactive = includeInactive;
        }

        public ComponentReference(bool includeInactive)
        {
            Id = string.Empty;
            IncludeInactive = includeInactive;
        }

        public static implicit operator T(ComponentReference<T> instance)
        {
            return instance?.Value;
        }
    }
}