using UnityEngine;

namespace GabrielBertasso.ShopSystem
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindFirstObjectByType<T>();
                }

                return s_instance;
            }
            protected set => s_instance = value;
        }

        private static T s_instance;


        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = (T)this;
        }
    }
}