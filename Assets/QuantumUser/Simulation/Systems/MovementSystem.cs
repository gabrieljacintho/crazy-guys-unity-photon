using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.GabrielBertasso
{
    [Preserve]
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerLink* PlayerLink;
            public Transform3D* Transform;
            public Movement* Movement;
            public KCC* KCC;
        }


        public override void Update(Frame frame, ref Filter filter)
        {
            if (!filter.KCC->IsActive)
            {
                return;
            }

            var kcc = filter.KCC;
            var movement = filter.Movement;
            var input = frame.GetPlayerInput(filter.PlayerLink->PlayerRef);

            if (filter.Transform->Position.Y < -15)
            {
                frame.Signals.PlayerFell(filter.Entity);
                return;
            }

            if (movement->JumpInProgress && kcc->IsGrounded)
            {
                frame.Events.Landed(filter.Entity);
                movement->JumpInProgress = false;
            }

            var lookRotation = FPQuaternion.Euler(0, input->LookRotation.Y, 0);
            var moveDirection = lookRotation * new FPVector3(input->MoveDirection.X, 0, input->MoveDirection.Y);

            if (movement->SetLookRotation)
            {
                kcc->SetLookRotation(input->LookRotation.X, input->LookRotation.Y);
            }
            else if (moveDirection != default)
            {
                var currentRotation = kcc->Data.TransformRotation;
                var targetRotation = FPQuaternion.LookRotation(moveDirection);
                var nextRotation = FPQuaternion.Lerp(currentRotation, targetRotation, movement->RotationSpeed * frame.DeltaTime);
                
                kcc->SetLookRotation(nextRotation);
            }

            kcc->SetInputDirection(moveDirection);

            if (input->Jump.WasPressed && kcc->IsGrounded)
            {
                kcc->Jump(FPVector3.Up * filter.Movement->JumpForce);
                movement->JumpInProgress = true;

                frame.Events.Jumped(filter.Entity);
            }
        }
    }
}
