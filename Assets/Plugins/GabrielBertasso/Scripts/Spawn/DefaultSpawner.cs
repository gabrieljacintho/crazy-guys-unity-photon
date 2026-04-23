using UnityEngine;

namespace GabrielBertasso.Spawn
{
    public class DefaultSpawner : ISpawner
    {
        public ISpawnHandle Spawn(GameObject prefab, Transform parent)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            return GetSpawnHandle(instance);
        }

        public ISpawnHandle Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject instance = Object.Instantiate(prefab, position, rotation);
            return GetSpawnHandle(instance);
        }

        public ISpawnHandle GetSpawnHandle(GameObject instance)
        {
            return new DefaultSpawnHandle(instance);
        }

        public void Despawn(GameObject instance)
        {
            Object.Destroy(instance);
        }
    }
}