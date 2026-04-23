using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Cache
{
    public static class AnimationParameterIds
    {
        private static readonly Dictionary<string, int> s_parameterIdByName = new Dictionary<string, int>();


        public static int GetId(string parameterName)
        {
            if (s_parameterIdByName.TryGetValue(parameterName, out int id))
            {
                return id;
            }

            id = Animator.StringToHash(parameterName);
            s_parameterIdByName.Add(parameterName, id);

            return id;
        }
    }
}