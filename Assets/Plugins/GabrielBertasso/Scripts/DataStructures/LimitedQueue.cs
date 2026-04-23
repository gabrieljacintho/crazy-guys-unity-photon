using System;
using GabrielBertasso.Extensions;

namespace GabrielBertasso.DataStructures
{
    [Serializable]
    public class LimitedQueue<T>
    {
        public T[] Items;

        public T this[int index] => Items[index];
        public int Length => Items.Length;


        public LimitedQueue(int length = 16)
        {
            Items = new T[length];
        }

        public void Enqueue(T item)
        {
            Items.ShiftRight();
            Items[0] = item;
        }

        public T Dequeue(int index = 0)
        {
            if (index >= Length)
            {
                Debug.LogError("Invalid index!");
                return default;
            }

            T item = Items[index];
            Items.ShiftLeft(index);

            return item;
        }

        public void Dequeue(T item)
        {
            if (!Items.Contains(item))
            {
                return;
            }

            int index = Array.IndexOf(Items, item);
            Dequeue(index);
        }

        public void ReEnqueue(T item)
        {
            if (!Items.Contains(item))
            {
                return;
            }

            Dequeue(item);
            Enqueue(item);
        }

        public T[] DequeueRange(int amount)
        {
            T[] items = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                items[i] = Dequeue();
            }

            return items;
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}