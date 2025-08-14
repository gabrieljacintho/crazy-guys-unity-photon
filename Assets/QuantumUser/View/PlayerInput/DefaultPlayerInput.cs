using UnityEngine;

namespace GabrielBertasso
{
    public class DefaultPlayerInput : PlayerInput
    {
        [SerializeField] private string _lookXAxisName = "Mouse X";
        [SerializeField] private string _lookYAxisName = "Mouse Y";
        [SerializeField] private string _moveXAxisName = "Horizontal";
        [SerializeField] private string _moveYAxisName = "Vertical";
        [SerializeField] private string _jumpButtonName = "Jump";
        [SerializeField] private string _sprintButtonName = "Sprint";


        protected override Vector2 GetLookRotationDelta()
        {
            return new Vector2(-Input.GetAxisRaw(_lookYAxisName), Input.GetAxisRaw(_lookXAxisName));
        }

        protected override Vector2 GetMoveDirection()
        {
            return new Vector2(Input.GetAxisRaw(_moveXAxisName), Input.GetAxisRaw(_moveYAxisName));
        }

        protected override bool CanJump()
        {
            return Input.GetButton(_jumpButtonName);
        }

        protected override bool CanSprint()
        {
            return Input.GetButton(_sprintButtonName);
        }
    }
}