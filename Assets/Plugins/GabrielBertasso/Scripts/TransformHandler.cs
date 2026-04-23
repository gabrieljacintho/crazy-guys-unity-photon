using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso
{
    public class TransformHandler : MonoBehaviour
    {
        [SerializeField] private bool _overridePosition;
        [ShowIf("_overridePosition")]
        [SerializeField] private Vector3 _position;
        [SerializeField] private bool _overrideRotation;
        [ShowIf("_overrideRotation")]
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private bool _overrideScale;
        [ShowIf("_overrideScale")]
        [SerializeField] private Vector3 _scale;
        [SerializeField] private Space _space;


        [Button]
        public void Apply()
        {
            if (_overridePosition)
            {
                if (_space == Space.World)
                {
                    transform.position = _position;
                }
                else
                {
                    transform.localPosition = _position;
                }
            }

            if (_overrideRotation)
            {
                if (_space == Space.World)
                {
                    transform.rotation = Quaternion.Euler(_rotation);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(_rotation);
                }
            }

            if (_overrideScale)
            {
                transform.localScale = _scale;
            }
        }
    }
}