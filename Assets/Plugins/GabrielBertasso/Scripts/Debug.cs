using System.Diagnostics;
using UnityEngine;

namespace GabrielBertasso
{
    public static class Debug
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, Object context = null)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, Object context = null)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, Object context = null)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(System.Exception exception, Object context = null)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
    }
}