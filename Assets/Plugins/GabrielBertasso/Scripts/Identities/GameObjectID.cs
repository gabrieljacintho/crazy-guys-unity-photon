using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    public class GameObjectID : MonoBehaviour
    {
        public const string PlayerID = "player";

        [SerializeField] private string _id;

        public static Dictionary<string, List<GameObject>> GameObjectsByID => GameObjectIDManager.Instance != null ?
            GameObjectIDManager.Instance.GameObjectsByID : new Dictionary<string, List<GameObject>>();


        private void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        public static GameObject FindGameObjectWithID(string id, bool includeInactive = false)
        {
            List<GameObject> gameObjects = FindGameObjectsWithID(id);

            if (gameObjects.Count == 0 && includeInactive)
            {
                gameObjects = FindGameObjectsWithID(id, true);
            }

            return gameObjects.Count > 0 ? gameObjects[0] : null;
        }

        public static List<GameObject> FindGameObjectsWithID(string id, bool includeInactive = false)
        {
            if (includeInactive)
            {
                RegisterAllObjects();
            }

            if (!GameObjectsByID.TryGetValue(id, out List<GameObject> gameObjects))
            {
                return new List<GameObject>();
            }

            if (includeInactive)
            {
                return gameObjects;
            }

            return gameObjects.FindAll(x => x.activeInHierarchy);
        }

        public static GameObject FindChildWithID(GameObject parent, string id, bool includeInactive = false)
        {
            List<GameObject> gameObjects = FindGameObjectsWithID(id);

            if (gameObjects.Count == 0 && includeInactive)
            {
                gameObjects = FindGameObjectsWithID(id, true);
            }

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.transform.IsChildOf(parent.transform))
                {
                    return gameObject;
                }
            }

            return null;
        }

        public static GameObject FindPlayerGameObject(bool includeInactive = true)
        {
            return FindGameObjectWithID(PlayerID, includeInactive);
        }

        public static T FindComponentInGameObjectWithID<T>(string id, bool includeInactive = false)
        {
            GameObject gameObject = FindGameObjectWithID(id, includeInactive);
            return gameObject != null ? gameObject.GetComponent<T>() : default;
        }

        public static T FindComponentInChildrenWithID<T>(string id, bool includeInactive = false)
        {
            GameObject gameObject = FindGameObjectWithID(id, includeInactive);
            return gameObject != null ? gameObject.GetComponentInChildren<T>(includeInactive) : default;
        }

        private void Register()
        {
            if (GameObjectsByID.TryGetValue(_id, out List<GameObject> gameObjects))
            {
                if (gameObjects.Contains(gameObject))
                {
                    return;
                }

                gameObjects.Add(gameObject);
            }
            else
            {
                GameObjectsByID.Add(_id, new List<GameObject>() { gameObject });
            }
        }

        private void Unregister()
        {
            if (!GameObjectsByID.TryGetValue(_id, out List<GameObject> gameObjects) || !gameObjects.Contains(gameObject))
            {
                return;
            }

            gameObjects.Remove(gameObject);
        }

        private static void RegisterAllObjects()
        {
            GameObjectID[] gameObjectIDs = FindObjectsByType<GameObjectID>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Array.ForEach(gameObjectIDs, gameObjectID => gameObjectID.Register());
        }
    }
}