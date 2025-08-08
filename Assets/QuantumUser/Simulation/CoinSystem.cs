using UnityEngine.Scripting;

namespace Quantum.GabrielBertasso
{
    [Preserve]
    public unsafe class CoinSystem : SystemMainThreadFilter<CoinSystem.Filter>, ISignalCoinCollected
    {
        public struct Filter
        {
            public EntityRef EntityRef;
            public Coin* Coin;
            public PhysicsCollider3D* Collider;
        }


        public override void Update(Frame frame, ref Filter filter)
        {
            if (filter.Coin->RefreshTimer.HasStoppedThisFrame(frame))
            {
                filter.Collider->Enabled = true;
            }
        }

        public void CoinCollected(Frame frame, EntityRef entity)
        {
            var coin = frame.Unsafe.GetPointer<Coin>(entity);
            coin->RefreshTimer = FrameTimer.FromSeconds(frame, coin->RefreshTime);

            var collider = frame.Unsafe.GetPointer<PhysicsCollider3D>(entity);
            collider->Enabled = false;
        }
    }
}