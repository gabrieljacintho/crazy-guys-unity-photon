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


        public override void Update(Frame f, ref Filter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}
