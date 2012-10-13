using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;

namespace CjClutter.OpenGl
{
    public class KeyDictionary
    {
        private readonly Dictionary<Key, bool> _keys;

        public KeyDictionary()
        {
            _keys = new Dictionary<Key, bool>();

            var keys = typeof (Key)
                .GetEnumValues()
                .Cast<Key>()
                .Distinct()
                .ToList();

            foreach (var key in keys)
            {
                _keys.Add(key, false);
            }
        }

        public bool this[Key key]
        {
            get { return _keys[key]; }
            set { _keys[key] = value; }
        }
    }
}