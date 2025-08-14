using UnityEngine;

namespace GabrielBertasso
{
    public class WebPlayerInput : DefaultPlayerInput
    {
        protected override Vector2 GetLookRotationDelta()
        {
            Vector2 value = base.GetLookRotationDelta();

#if !UNITY_EDITOR
            if (Mathf.Abs(value.x) > 45 || Mathf.Abs(value.y) > 45)
            {
                // Prevent glitch in Chrome with high polling mice where cursor jumps rapidly from time to time
                value = default;
            }

            // Sensitivity in WebGL builds on desktop is much higher for some reason, decrease it
            value *= 0.5f;
#endif

            return value;
        }
    }
}