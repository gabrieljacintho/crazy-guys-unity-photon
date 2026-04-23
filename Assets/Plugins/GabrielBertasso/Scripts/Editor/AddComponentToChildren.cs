using GabrielBertasso.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso
{
    public abstract class AddComponentToChildren<T> : MonoBehaviour where T : Component
    {
        [Button]
        public void Add()
        {
            AddRecursively(transform);
        }

        [Button]
        public void Remove()
        {
            RemoveRecursively(transform);
        }

        private void AddRecursively(Transform transform)
        {
            transform.gameObject.GetOrAddComponent<T>();

            foreach (Transform child in transform)
            {
                AddRecursively(child);
            }
        }

        private void RemoveRecursively(Transform transform)
        {
            if (transform.gameObject.TryGetComponent(out T joint))
            {
#if UNITY_EDITOR
                DestroyImmediate(joint);
#else
                Destroy(joint);
#endif
            }

            foreach (Transform child in transform)
            {
                RemoveRecursively(child);
            }
        }
    }
}