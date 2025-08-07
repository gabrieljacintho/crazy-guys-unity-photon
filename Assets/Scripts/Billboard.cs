using UnityEngine;

namespace GabrielBertasso
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraTransform;


        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.rotation = _cameraTransform.rotation;
        }
    }
}