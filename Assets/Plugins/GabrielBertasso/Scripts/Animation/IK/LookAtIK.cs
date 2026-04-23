using GabrielBertasso.Identities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GabrielBertasso.Animation.IK
{
    public class LookAtIK : MonoBehaviour
    {
        [SerializeField] private bool _isActive = true;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObjectReference _target = new GameObjectReference("player-camera", true);
        [SerializeField] private float _speed = 1f;

        private float _weight;

        public bool IsActive
        {
            get => _isActive;
            [Button]
            set => _isActive = value;
        }


        private void OnAnimatorIK(int layerIndex)
        {
            if (_target.Value != null)
            {
                _animator.SetLookAtPosition(_target.Value.transform.position);
            }

            float target = _isActive && _target.Value != null ? 1f : 0f;
            _weight = Mathf.MoveTowards(_weight, target, _speed * Time.deltaTime);
            _animator.SetLookAtWeight(_weight);
        }
    }
}