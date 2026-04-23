using UnityEngine;

namespace GabrielBertasso
{
    public class DestroyGameObject : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;


        public void Destroy()
        {
            Object.Destroy(_gameObject);
        }

        public void Destroy(float delay)
        {
            Object.Destroy(_gameObject, delay);
        }

        public void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}