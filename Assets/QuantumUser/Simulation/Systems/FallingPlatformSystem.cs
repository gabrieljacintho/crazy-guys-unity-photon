using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.GabrielBertasso
{
    [Preserve]
    public unsafe class FallingPlatformSystem : SystemMainThreadFilter<FallingPlatformSystem.Filter>, ISignalOnCollisionEnter3D
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FallingPlatform* Platform;
            public Transform3D* Transform;
            public PhysicsBody3D* PhysicsBody;
            public PhysicsCollider3D* Collider;
        }


        public override void OnInit(Frame frame)
        {
            foreach (var pair in frame.Unsafe.GetComponentBlockIterator<FallingPlatform>())
            {
                pair.Component->OriginalPosition = frame.Unsafe.GetPointer<Transform3D>(pair.Entity)->Position;
            }
        }
        
        public override void Update(Frame frame, ref Filter filter)
        {
            if (filter.Platform->FallTimer.HasStoppedThisFrame(frame))
            {
                filter.Collider->Layer = UnityEngine.LayerMask.NameToLayer("Ignore Raycast");
                filter.PhysicsBody->GravityScale = 1;
                filter.PhysicsBody->AddLinearImpulse(FPVector3.Down * 30);

                filter.Platform->ResetTimer = FrameTimer.FromSeconds(frame, filter.Platform->ResetTime);
                frame.Events.PlatformFell(filter.Entity);
            }
            else if (filter.Platform->ResetTimer.HasStoppedThisFrame(frame))
            {
                filter.Collider->Layer = 0;
                filter.PhysicsBody->GravityScale = 0;
                filter.PhysicsBody->Velocity = default;
                filter.Transform->Position = filter.Platform->OriginalPosition;

                filter.Platform->FallTimer = default;
                filter.Platform->ResetTimer = default;
            }
        }

        public void OnCollisionEnter3D(Frame frame, CollisionInfo3D info)
        {
            if (!frame.Has<Player>(info.Other))
            {
                return;
            }

            if (!frame.Unsafe.TryGetPointer(info.Entity, out FallingPlatform* fallingPlatform) || fallingPlatform->FallTimer.IsSet)
            {
                return;
            }

            fallingPlatform->FallTimer = FrameTimer.FromSeconds(frame, fallingPlatform->FallDelay);
        }
    }
}