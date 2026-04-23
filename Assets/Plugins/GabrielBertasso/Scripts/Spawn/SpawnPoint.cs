using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.Spawn
{
    [Serializable]
    public class SpawnPoint
    {
        public Transform Transform;
        [Range(0f, 1f)] public float Weight = 1f;
        [HideInInspector] public ISpawnHandle Handle;

        [ShowInInspector, ReadOnly] public GameObject Instance => Handle?.Instance;


        public SpawnPoint()
        {
            Transform = null;
            Weight = 1f;
            Handle = null;
        }

        public SpawnPoint(Transform transform, float weight = 1f, GameObject instance = null)
        {
            Transform = transform;
            Weight = weight;
            Handle = instance != null ? SpawnProvider.Instance.GetSpawnHandle(instance) : null;
        }
    }
}