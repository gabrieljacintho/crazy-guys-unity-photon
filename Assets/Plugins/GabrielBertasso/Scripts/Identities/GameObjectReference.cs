using System;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    [Serializable]
    public class GameObjectReference
    {
        [SerializeField] private GameObject _gameObject;
        public string Id;
        public bool IncludeInactive;

        public GameObject Value
        {
            get
            {
                if (_gameObject != null)
                {
                    return _gameObject;
                }

                _gameObject = GameObjectID.FindGameObjectWithID(Id, IncludeInactive);

                return _gameObject;
            }
            set => _gameObject = value;
        }


        public GameObjectReference(string id, bool includeInactive = false)
        {
            Id = id;
            IncludeInactive = includeInactive;
        }

        public static implicit operator GameObject(GameObjectReference instance)
        {
            return instance.Value;
        }
    }
}