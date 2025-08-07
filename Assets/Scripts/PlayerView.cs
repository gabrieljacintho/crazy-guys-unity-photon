using Quantum;
using TMPro;
using UnityEngine;

namespace GabrielBertasso
{
    public class PlayerView : QuantumEntityViewComponent<SceneContext>
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private Transform _cameraHandle;
        [Tooltip("Root transform for squash/stretch effects")]
        [SerializeField] private Transform _scalingRoot;
        [SerializeField] private TextMeshProUGUI _nicknameText;

        [Header("Sounds")]
        [SerializeField] private AudioSource _footstepAudioSource;
        [SerializeField] private AudioClip _jumpAudioClip;
        [SerializeField] private AudioClip _landAudioClip;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _dustParticles;

        private Transform _cameraTransform;


        public override void OnActivate(Frame frame)
        {
            QuantumEvent.Subscribe<EventJumped>(this, OnJumped);
            QuantumEvent.Subscribe<EventLanded>(this, OnLanded);

            var playerLink = GetPredictedQuantumComponent<PlayerLink>();
            if (Game.PlayerIsLocal(playerLink.PlayerRef))
            {
                ViewContext.LocalPlayer = playerLink.PlayerRef;
                ViewContext.LocalPlayerEntity = EntityRef;
            }
            else
            {
                var playerData = frame.GetPlayerData(playerLink.PlayerRef);
                _nicknameText.text = playerData != null ? playerData.PlayerNickname : string.Empty;
            }

            _cameraTransform = Camera.main.transform;
        }

        public override void OnUpdateView()
        {
            var kcc = GetPredictedQuantumComponent<KCC>();
            UpdateAnimator(kcc);
            UpdateFootstepAudioSource(kcc);
            UpdateScalingRoot();
            UpdateDustParticles(kcc);
        }

        public override void OnLateUpdateView()
        {
            if (EntityRef != ViewContext.LocalPlayerEntity)
            {
                return;
            }

            UpdateCamera();
        }

        #region OnUpdateView Methods

        private void UpdateAnimator(KCC kcc)
        {
            _animator.SetFloat(AnimatorId.MovementAmount, kcc.RealSpeed.AsFloat / kcc.Data.KinematicSpeed.AsFloat);
            _animator.SetBool(AnimatorId.IsGrounded, kcc.IsGrounded);
        }

        private void UpdateFootstepAudioSource(KCC kcc)
        {
            _footstepAudioSource.enabled = kcc.IsGrounded && kcc.RealSpeed > 1;
            _footstepAudioSource.pitch = kcc.RealSpeed > 6 ? 1.5f : 1f;
        }

        private void UpdateScalingRoot()
        {
            _scalingRoot.localScale = Vector3.Lerp(_scalingRoot.localScale, Vector3.one, Time.deltaTime * 8f);
        }

        private void UpdateDustParticles(KCC kcc)
        {
            var emission = _dustParticles.emission;
            emission.enabled = kcc.IsGrounded && kcc.RealSpeed > 1;
        }

        #endregion

        private void UpdateCamera()
        {
            _cameraPivot.rotation = Quaternion.Euler(_input.LookRotation);
            _cameraTransform.SetPositionAndRotation(_cameraHandle.position, _cameraHandle.rotation);
        }

        private void OnJumped(EventJumped callback)
        {
            if (callback.Entity != EntityRef)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(_jumpAudioClip, transform.position, 1f);
            _scalingRoot.localScale = new Vector3(0.5f, 1.5f, 0.5f);
        }

        private void OnLanded(EventLanded callback)
        {
            if (callback.Entity != EntityRef)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(_landAudioClip, transform.position, 1f);
            _scalingRoot.localScale = new Vector3(1.25f, 0.75f, 1.25f);
        }
    }
}