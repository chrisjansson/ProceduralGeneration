using System.Collections.Generic;
using System.Linq;

namespace CjClutter.Commons.Collections
{
    public class MultiValueDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TValue> _equalityComparer;
        private readonly Dictionary<TKey, List<TValue>> _dictionary;

        public MultiValueDictionary()
            : this(EqualityComparer<TValue>.Default) { }

        public MultiValueDictionary(IEqualityComparer<TValue> equalityComparer)
        {
            _equalityComparer = equalityComparer;
            _dictionary = new Dictionary<TKey, List<TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            var containsKey = _dictionary.ContainsKey(key);
            if (containsKey)
            {
                _dictionary[key].Add(value);
            }
            else
            {
                var valueCollection = new List<TValue> { value };
                _dictionary.Add(key, valueCollection);
            }
        }

        public void Clear()
        {
            foreach (var value in _dictionary.Values)
            {
                value.Clear();
            }

            _dictionary.Clear();
        }

        public bool Contains(TKey key, TValue item)
        {
            if (!ContainsKey(key))
            {
                return false;
            }

            var container = _dictionary[key];
            return container.Contains(item, _equalityComparer);
        }

        public bool Remove(TKey key, TValue item)
        {
            if (!ContainsKey(key))
            {
                return false;
            }

            var container = _dictionary[key];
            var numberOfItemsRemoved = container.RemoveAll(x => _equalityComparer.Equals(x, item));

            return numberOfItemsRemoved > 0;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public IEnumerable<TValue> this[TKey key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value.ToList(); }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        //public ICollection<List<TValue>> Values
        //{
        //    get { return _dictionary.Values; }
        //}

        //public bool TryGetValue(TKey key, out TValue value)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
