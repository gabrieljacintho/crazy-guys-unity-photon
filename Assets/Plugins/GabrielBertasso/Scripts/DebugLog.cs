using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso
{
    [Serializable]
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    public class DebugLog : MonoBehaviour
    {
        [SerializeField] private LogType _logType;
        [TextArea(1, 5), HideLabel]
        [SerializeField] private string _message;


        [Button]
        public void Log()
        {
            switch (_logType)
            {
                case LogType.Log:
                    Log(_message);
                    break;

                case LogType.Warning:
                    LogWarning(_message);
                    break;

                case LogType.Error:
                    LogError(_message);
                    break;
            }
        }

        public void Log(string message)
        {
            Debug.Log(message, this);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(message, this);
        }

        public void LogError(string message)
        {
            Debug.LogError(message, this);
        }
    }
}