using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using GabrielBertasso.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GabrielBertasso.Spawn
{
    public class Spawner : MonoBehaviour
    {
        [Serializable]
        public struct PrefabData
        {
            public GameObject Prefab;
        }

        [SerializeField] private List<PrefabData> _prefabs;
        [SerializeField] private List<SpawnPoint> _spawnPoints;
        [SerializeField] private bool _attach;

        [Header("Settings")]
        [SerializeField] private IntReference _minRandomQuantity = new IntReference(1);
        [SerializeField] private IntReference _maxRandomQuantity = new IntReference(5);
        [Tooltip("Set negative to infinity.")]
        [SerializeField] private IntReference _maxActiveInstances = new IntReference(-1);
        [SerializeField] private bool _avoidance;

        private List<float> _weights;

        public List<float> Weights
        {
            get
            {
                if (_weights == null)
                {
                    _weights = new List<float>();
                    foreach (var spawnPoint in _spawnPoints)
                    {
                        _weights.Add(spawnPoint.Weight);
                    }
                }

                return _weights;
            }
        }

        public Action Changed;


        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (_spawnPoints == null || _spawnPoints.Count == 0)
            {
                _spawnPoints = new List<SpawnPoint>() { new SpawnPoint(transform) };
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public GameObject Spawn()
        {
            if (_maxActiveInstances.Value >= 0 && GetActiveInstances().Count >= _maxActiveInstances.Value)
            {
                return null;
            }

            if (!TryGetRandomSpawnPoint(out SpawnPoint spawnPoint))
            {
                return null;
            }

            PrefabData option = _prefabs[UnityEngine.Random.Range(0, _prefabs.Count)];
            Transform optionTransform = spawnPoint.Transform;

            ISpawnHandle handle;
            if (_attach)
            {
                handle = SpawnProvider.Instance.Spawn(option.Prefab, optionTransform);
            }
            else
            {
                handle = SpawnProvider.Instance.Spawn(option.Prefab, optionTransform.position, optionTransform.rotation);
            }

            spawnPoint.Handle = handle;
            Changed?.Invoke();

            handle.Released += () =>
            {
                spawnPoint.Handle = null;
                Changed?.Invoke();
            };

            return handle.Instance;
        }

        [Button]
        public void InvokeSpawn()
        {
            Spawn();
        }

        public async Task<List<GameObject>> Spawn(int quantity)
        {
            List<GameObject> instances = new List<GameObject>();

            for (int i = 0; i < quantity; i++)
            {
                GameObject instance = Spawn();
                if (instance != null)
                {
                    instances.Add(instance);
                    await Task.Yield();
                }
                else
                {
                    break;
                }
            }

            return instances;
        }

        public async void InvokeSpawn(int quantity)
        {
            await Spawn(quantity);
        }

        public async Task<List<GameObject>> SpawnRandomQuantity()
        {
            int quantity = UnityEngine.Random.Range(_minRandomQuantity.Value, _maxRandomQuantity.Value + 1);
            return await Spawn(quantity);
        }

        public async void InvokeSpawnRandomQuantity()
        {
            await SpawnRandomQuantity();
        }

        private bool TryGetRandomSpawnPoint(out SpawnPoint spawnPoint)
        {
            spawnPoint = null;

            if (_spawnPoints.Count == 1)
            {
                spawnPoint = _spawnPoints[0];
                return true;
            }

            List<SpawnPoint> emptySpawnPoints;
            if (_avoidance)
            {
                emptySpawnPoints = _spawnPoints.FindAll(x => x.Instance == null);

                if (emptySpawnPoints.Count > 0)
                {
                    List<float> weights = new List<float>();
                    foreach (var emptySpawnPoint in emptySpawnPoints)
                    {
                        weights.Add(emptySpawnPoint.Weight);
                    }

                    spawnPoint = emptySpawnPoints[WeightedRandom.PickIndex(weights)];
                }
            }
            else
            {
                spawnPoint = _spawnPoints[WeightedRandom.PickIndex(Weights)];
            }

            return spawnPoint != null;
        }

        public async Task<List<GameObject>> RespawnAll()
        {
            int quantity = Mathf.Max(GetActiveInstances().Count, 1);
            DespawnAll();
            return await Spawn(quantity);
        }

        public async void InvokeRespawnAll()
        {
            await RespawnAll();
        }

        [Button]
        public void DespawnAll()
        {
            List<ISpawnHandle> instances = new List<ISpawnHandle>(GetActiveInstances());
            foreach (var instance in instances)
            {
                SpawnProvider.Instance.Despawn(instance.Instance);
            }
        }

        public List<ISpawnHandle> GetActiveInstances()
        {
            List<ISpawnHandle> instances = new List<ISpawnHandle>();
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint.Instance != null)
                {
                    instances.Add(spawnPoint.Handle);
                }
            }
            return instances;
        }
    }
}