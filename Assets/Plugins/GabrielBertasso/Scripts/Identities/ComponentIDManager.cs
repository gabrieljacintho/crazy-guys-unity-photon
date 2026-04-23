using System.Collections.Generic;
using GabrielBertasso.Patterns;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    public class ComponentIDManager : Singleton<ComponentIDManager>
    {
        private Dictionary<string, List<Component>> _componentsByID = new();

        public Dictionary<string, List<Component>> ComponentsByID => _componentsByID;
    }
}