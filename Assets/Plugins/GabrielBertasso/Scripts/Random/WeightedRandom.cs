using System.Collections.Generic;

namespace GabrielBertasso.Random
{
    public static class WeightedRandom
    {
        // weights: non-negative floats. returns index in [0, weights.Count-1], or -1 if no valid weights.
        public static int PickIndex(IList<float> weights)
        {
            int n = weights.Count;
            float total = 0f;
            for (int i = 0; i < n; i++)
                total += weights[i];

            if (total <= 0f) return -1; // no valid weight

            float r = UnityEngine.Random.value * total; // [0, total)
            float cum = 0f;
            for (int i = 0; i < n; i++)
            {
                cum += weights[i];
                if (r < cum) return i;
            }
            return n - 1; // fallback due to floating point
        }
    }
}