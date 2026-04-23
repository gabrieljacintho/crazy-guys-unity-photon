using GabrielBertasso.Patterns;
using UnityEngine;

namespace GabrielBertasso
{
    public class UpdateManager : PersistentSingleton<UpdateManager>
    {
        public static int FixedUpdateCount { get; private set; }


        private void FixedUpdate()
        {
            FixedUpdateCount++;

            if (FixedUpdateCount == int.MaxValue)
            {
                FixedUpdateCount = 0;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (s_instance == null)
            {
                s_instance = GetInstance();
            }
        }
    }
}