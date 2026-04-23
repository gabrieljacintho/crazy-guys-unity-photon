using GabrielBertasso.Helpers;
using UnityEngine;

namespace GabrielBertasso.Patterns
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = GetInstance();
                }

                return s_instance;
            }
        }

        public static bool HasInstance => s_instance != null;
        protected static T s_instance;


        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected static T GetInstance()
        {
            if (ApplicationManager.IsQuitting)
            {
                return null;
            }

            T instance = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (instance != null)
            {
                return instance;
            }

            return GameObjectHelper.CreateGameObjectWithComponent<T>();
        }
    }
}
