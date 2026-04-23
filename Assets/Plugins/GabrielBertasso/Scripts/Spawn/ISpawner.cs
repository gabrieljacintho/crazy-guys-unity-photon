using UnityEngine;

namespace GabrielBertasso.Spawn
{
    public interface ISpawner
    {
        ISpawnHandle Spawn(GameObject prefab, Transform parent);

        ISpawnHandle Spawn(GameObject prefab, Vector3 position, Quaternion rotation);

        ISpawnHandle GetSpawnHandle(GameObject instance);

        void Despawn(GameObject instance);
    }
}