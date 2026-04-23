using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask.value == (layerMask.value | (1 << layer));
        }

        public static bool IsEmpty(this LayerMask layerMask)
        {
            return layerMask.value == 0;
        }
    }
}
