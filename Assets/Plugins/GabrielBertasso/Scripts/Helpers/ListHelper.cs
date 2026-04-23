using System.Collections.Generic;

namespace GabrielBertasso.Helpers
{
    public static class ListHelper
    {
        public static bool DoHaveSameItems<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null || list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            var dict = new Dictionary<T, int>();

            // Count elements in list1
            for (int i = 0; i < list1.Count; i++)
            {
                T item = list1[i];
                if (dict.ContainsKey(item))
                    dict[item]++;
                else
                    dict[item] = 1;
            }

            // Subtract counts using list2
            for (int i = 0; i < list2.Count; i++)
            {
                T item = list2[i];
                if (!dict.ContainsKey(item))
                    return false;

                dict[item]--;
                if (dict[item] < 0)
                    return false;
            }

            return true;
        }
    }
}