using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Camera
{
    public class SyncCameras : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _targetCamera;
        [SerializeField] private List<UnityEngine.Camera> _camerasToSync;


        private void LateUpdate()
        {
            foreach (UnityEngine.Camera camera in _camerasToSync)
            {
                camera.fieldOfView = _targetCamera.fieldOfView;
            }
        }
    }
}