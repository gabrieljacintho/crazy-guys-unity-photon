using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.DataStructures
{
    [Serializable]
    public class DictionaryList<TKey, TValue>
    {
        [SerializeField] private List<KeyValue<TKey, TValue>> _list = new List<KeyValue<TKey, TValue>>();

        public List<KeyValue<TKey, TValue>> List
        {
            get
            {
                _list ??= new List<KeyValue<TKey, TValue>>();
                return _list;
            }
        }
        public TValue this[TKey key]
        {
            get
            {
                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].Key.Equals(key))
                    {
                        return List[i].Value;
                    }
                }
                return default;
            }
            set => Add(key, value);
        }


        public DictionaryList() { }

        public DictionaryList(IEnumerable<KeyValue<TKey, TValue>> items)
        {
            if (items != null)
            {
                _list = new List<KeyValue<TKey, TValue>>(items);
            }
        }

        public DictionaryList(IEnumerable<(TKey key, TValue value)> items)
        {
            if (items != null)
            {
                _list = new List<KeyValue<TKey, TValue>>();
                foreach (var (key, value) in items)
                {
                    _list.Add(new KeyValue<TKey, TValue>(key, value));
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            var keyValue = new KeyValue<TKey, TValue>(key, value);
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Key.Equals(key))
                {
                    List[i] = keyValue;
                    return;
                }
            }
            List.Add(keyValue);
        }

        public void Remove(TKey key)
        {
            int i = FindIndex(key);
            if (i > -1)
            {
                List.RemoveAt(i);
            }
        }

        public int FindIndex(TKey key)
        {
            return List.FindIndex(x => x.Key.Equals(key));
        }

        public bool Exists(TKey key)
        {
            return List.Exists(x => x.Key.Equals(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool Predicate(KeyValue<TKey, TValue> keyValue)
            {
                return key != null ? keyValue.Key != null && keyValue.Key.Equals(key) : keyValue.Key == null;
            }

            if (List.Exists(Predicate))
            {
                value = List.Find(Predicate).Value;
                return true;
            }

            value = default;
            return false;
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

            foreach (var item in List)
            {
                dictionary.Add(item.Key, item.Value);
            }

            return dictionary;
        }
    }
}