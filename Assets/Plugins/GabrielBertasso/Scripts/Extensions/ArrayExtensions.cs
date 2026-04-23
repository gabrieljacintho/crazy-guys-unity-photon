using System;
using System.Collections.Generic;
using Unity.Collections;

namespace GabrielBertasso.Extensions
{
    public static class ArrayExtensions
    {
        public static void ShiftLeft<T>(this T[] array, int startIndex = 0, int amount = 1)
        {
            for (int i = startIndex; i < array.Length; i++)
            {
                array[i] = i + amount < array.Length ? array[i + amount] : default;
            }
        }

        public static void ShiftRight<T>(this T[] array, int startIndex = 0, int amount = 1)
        {
            for (int i = array.Length - 1; i >= startIndex; i--)
            {
                array[i] = i - amount >= 0 ? array[i - amount] : default;
            }
        }

        public static bool Any<T>(this T[] array, Predicate<T> match)
        {
            foreach (T t in array)
            {
                if (match.Invoke(t))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<T> ToList<T>(this T[] array)
        {
            List<T> list = new List<T>();
            foreach (T type in array)
            {
                list.Add(type);
            }

            return list;
        }

        public static List<T> ToList<T>(this NativeArray<T> array) where T : struct
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            if (item == null)
            {
                return Array.Exists(array, x => x == null);
            }

            return Array.Exists(array, x => x != null && x.Equals(item));
        }

        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }
    }
}