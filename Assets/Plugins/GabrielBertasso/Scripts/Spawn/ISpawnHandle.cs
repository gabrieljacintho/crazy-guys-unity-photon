using System;
using UnityEngine;

namespace GabrielBertasso.Spawn
{
    public interface ISpawnHandle
    {
        GameObject Instance { get; }

        event Action Released;
    }
}
