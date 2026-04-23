using UnityEngine;

namespace GabrielBertasso
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
    }
}