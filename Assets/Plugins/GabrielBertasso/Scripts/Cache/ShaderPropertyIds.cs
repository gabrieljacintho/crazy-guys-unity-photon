using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Cache
{
    public static class ShaderPropertyIds
    {
        private static readonly Dictionary<string, int> s_propertyIdByName = new Dictionary<string, int>();

        public static readonly int MainTex = GetId("_MainTex");
        public static readonly int Color = GetId("_Color");
        public static readonly int EmissionColor = GetId("_EmissionColor");


        public static int GetId(string parameterName)
        {
            if (s_propertyIdByName.TryGetValue(parameterName, out int id))
            {
                return id;
            }

            id = Shader.PropertyToID(parameterName);
            s_propertyIdByName.Add(parameterName, id);

            return id;
        }
    }
}