using System;
using System.Collections.Generic;

namespace GabrielBertasso.DataStructures
{
    [Serializable]
    public class SerializedList<T>
    {
        public List<T> Items = new List<T>();

        public T this[int index] => Items[index];
        public int Count => Items.Count;


        public SerializedList()
        {
            Items = new List<T>();
        }
    }
}