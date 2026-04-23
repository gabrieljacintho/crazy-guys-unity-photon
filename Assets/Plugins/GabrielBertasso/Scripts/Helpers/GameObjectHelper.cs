using UnityEngine;

namespace GabrielBertasso.Helpers
{
    public static class GameObjectHelper
    {
        public static T CreateGameObjectWithComponent<T>() where T : Component
        {
            GameObject gameObject = new GameObject(typeof(T).Name);
            return gameObject.AddComponent<T>();
        }
    }
}