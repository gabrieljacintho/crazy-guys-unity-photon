using Sirenix.OdinInspector;
using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace GabrielBertasso.Animation
{
    [Serializable]
    public enum AnimatorParameterType
    {
        Float,
        Int,
        Bool,
        Trigger
    }

    [RequireComponent(typeof(Animator))]
    public class AnimatorHandler : MonoBehaviour
    {
        [SerializeField] private AnimatorParameterType _parameterType;
        [SerializeField] private string _parameterName;
        [ShowIf("@_parameterType == AnimatorParameterType.Float")]
        [SerializeField] private FloatReference _floatValue;
        [ShowIf("@_parameterType == AnimatorParameterType.Int")]
        [SerializeField] private IntReference _intValue;
        [ShowIf("@_parameterType == AnimatorParameterType.Bool")]
        [SerializeField] private BoolReference _boolValue;
        [SerializeField] private bool _setParameterOnAwake;
        [SerializeField] private bool _setParameterOnEnable;

        private Animator _animator;

        private int _parameterHash;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _parameterHash = Animator.StringToHash(_parameterName);

            if (_setParameterOnAwake)
            {
                SetParameter();
            }
        }

        private void OnEnable()
        {
            if (_setParameterOnEnable)
            {
                SetParameter();
            }
        }

        [Button]
        public void SetParameter()
        {
            switch (_parameterType)
            {
                case AnimatorParameterType.Float:
                    _animator.SetFloat(_parameterHash, _floatValue.Value);
                    break;

                case AnimatorParameterType.Int:
                    _animator.SetInteger(_parameterHash, _intValue.Value);
                    break;

                case AnimatorParameterType.Bool:
                    _animator.SetBool(_parameterHash, _boolValue.Value);
                    break;

                case AnimatorParameterType.Trigger:
                    _animator.SetTrigger(_parameterHash);
                    break;
            }
        }

        public void SetParameterName(string value)
        {
            _parameterName = value;
            _parameterHash = Animator.StringToHash(_parameterName);
        }

        public void SetFloat(float value)
        {
            _animator.SetFloat(_parameterHash, value);
        }

        public void SetBool(bool value)
        {
            _animator.SetBool(_parameterHash, value);
        }
        
        public void SetInteger(int value)
        {
            _animator.SetInteger(_parameterHash, value);
        }
        
        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);
        }
    }
}