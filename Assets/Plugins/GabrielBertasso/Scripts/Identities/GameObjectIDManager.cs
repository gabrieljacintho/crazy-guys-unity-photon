using System.Collections.Generic;
using GabrielBertasso.Patterns;
using UnityEngine;

namespace GabrielBertasso.Identities
{
    public class GameObjectIDManager : Singleton<GameObjectIDManager>
    {
        private Dictionary<string, List<GameObject>> _gameObjectsByID = new();

        public Dictionary<string, List<GameObject>> GameObjectsByID => _gameObjectsByID;
    }
}