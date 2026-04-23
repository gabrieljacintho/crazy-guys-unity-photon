using UnityEngine;

namespace GabrielBertasso
{
    public class QuitGame : MonoBehaviour
    {
#if UNITY_WEBGL
        private void Awake()
        {
            gameObject.SetActive(false);
        }
#endif
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}