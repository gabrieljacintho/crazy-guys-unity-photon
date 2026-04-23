using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Patterns
{
    public class SingletonGameObject : MonoBehaviour
    {
        [SerializeField] private string _id = System.Guid.NewGuid().ToString();

        public static Dictionary<string, GameObject> s_instances = new Dictionary<string, GameObject>();


        protected virtual void Awake()
        {
            if (s_instances.ContainsKey(_id))
            {
                Destroy(gameObject);
            }
            else
            {
                s_instances.Add(_id, gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (s_instances.ContainsValue(gameObject))
            {
                s_instances.Remove(_id);
            }
        }
    }
}