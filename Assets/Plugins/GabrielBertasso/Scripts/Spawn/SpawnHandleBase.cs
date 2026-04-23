using System;
using UnityEngine;

namespace GabrielBertasso.Spawn
{
    public abstract class SpawnHandleBase : ISpawnHandle
    {
        public GameObject Instance { get; protected set; }
        public event Action Released;

        protected void InvokeReleased()
        {
            Released?.Invoke();
        }
    }
}
