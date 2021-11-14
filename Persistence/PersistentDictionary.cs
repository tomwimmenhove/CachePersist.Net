using System.Collections;
using System.Collections.Generic;

namespace Serialization.Persistence
{
    public class PersistentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionaryStore<TKey, TValue> _store;

        public ICollection<TKey> Keys => _store.Dictionary.Keys;
        public ICollection<TValue> Values => _store.Dictionary.Values;
        public int Count => _store.Dictionary.Count;

        public PersistentDictionary(IDictionaryStore<TKey, TValue> store)
        {
            _store = store;
        }

        public TValue this[TKey key]
        {
            get => _store.Dictionary[key];
            set
            {
                _store.Dictionary[key] = value;

                _store.Save();
            }
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _store.Dictionary.GetEnumerator();
        public bool IsReadOnly => _store.Dictionary.IsReadOnly;
        public void Add(KeyValuePair<TKey, TValue> kvp)
        {
            _store.Dictionary.Add(kvp);
            _store.Save();
        }

        public void Add(TKey key, TValue value)
        {
            _store.Dictionary.Add(key, value);
            _store.Save();
        }

        public bool ContainsKey(TKey key) => _store.Dictionary.ContainsKey(key);
        public bool ContainsKey(KeyValuePair<TKey, TValue> kvp) => _store.Dictionary.Contains(kvp);
        public bool Contains(KeyValuePair<TKey, TValue> kvp) => _store.Dictionary.Contains(kvp);
        public bool Remove(TKey key)
        {
            if (_store.Dictionary.Remove(key))
            {
                _store.Save();
                return true;
            }
            return false;
        }
        public bool Remove(KeyValuePair<TKey, TValue> kvp)
        {
            if (_store.Dictionary.Remove(kvp))
            {
                _store.Save();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _store.Dictionary.Clear();
            _store.Save();
        }

        public bool TryGetValue(TKey key, out TValue value) => _store.Dictionary.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) => _store.Dictionary.CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => _store.Dictionary.GetEnumerator();
    }
}
