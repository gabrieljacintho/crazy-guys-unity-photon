namespace GabrielBertasso.Helpers
{
    public static class SymbolsHelper
    {
        public static bool IsInDevelopment()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEVELOPMENT
            return true;
#else
            return false;
#endif  
        }

        public static bool IsInMobilePlatform()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return UnityEngine.Application.isMobilePlatform;
#endif  
        }
    }
}