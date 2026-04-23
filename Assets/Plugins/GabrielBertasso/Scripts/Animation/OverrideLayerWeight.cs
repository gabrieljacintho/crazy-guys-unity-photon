using UnityEngine;

namespace GabrielBertasso.Animation
{
    public class OverrideLayerWeight : StateMachineBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float _weight = 1;

        private float _previousWeight;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _previousWeight = animator.GetLayerWeight(layerIndex);
            animator.SetLayerWeight(layerIndex, _weight);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(layerIndex, _previousWeight);
        }
    }
}