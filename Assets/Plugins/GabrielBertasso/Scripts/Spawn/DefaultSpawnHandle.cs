using UnityEngine;

namespace GabrielBertasso.Spawn
{
    public class DefaultSpawnHandle : SpawnHandleBase
    {
        public DefaultSpawnHandle(GameObject gameObject)
        {
            Instance = gameObject;
        }
    }
}