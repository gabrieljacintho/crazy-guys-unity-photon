#if I2_LOCALIZATION
using I2.Loc;
#endif
using Sirenix.OdinInspector;
#if STEAMWORKS
using Steamworks;
#endif
using UnityEngine;

namespace GabrielBertasso
{
    public class OpenURL : MonoBehaviour
    {
#if I2_LOCALIZATION
        [SerializeField] private bool _useLocalizedURL;
        [ShowIf("_useLocalizedURL")]
        [SerializeField] private LocalizedString _localizedURL;
        [HideIf("_useLocalizedURL")]
#endif
        [SerializeField] private string _url;


        public void Open()
        {
#if I2_LOCALIZATION
            string url = _useLocalizedURL ? _localizedURL : _url;
#else
            string url = _url;
#endif

            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("URL is empty!", this);
                return;
            }

            Open(url);
        }

        public static void Open(string url)
        {
#if STEAMWORKS
            try
            {
                SteamFriends.OpenWebOverlay(url);
            }
            catch
            {
                Application.OpenURL(url);
            }
#else
            Application.OpenURL(url);
#endif
        }
    }
}