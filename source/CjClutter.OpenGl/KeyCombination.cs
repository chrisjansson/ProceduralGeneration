using System;
using OpenTK.Input;
using System.Linq;

namespace CjClutter.OpenGl
{
    public class KeyCombination
    {
        public KeyCombination(params Key[] keys)
        {
            Keys = keys;
        }

        public Key[] Keys { get; private set; }

        public static KeyCombination operator &(KeyCombination a, KeyCombination b)
        {
            var left = a.Keys;
            var right = b.Keys;

            var combinedKeys = left.Union(right)
                .Distinct()
                .ToArray();

            return new KeyCombination(combinedKeys);
        }

        public static bool operator false(KeyCombination a)
        {
            if(a == null)
            {
                throw new System.NotImplementedException();
            }

            return false;
        }

        public static bool operator true(KeyCombination a)
        {
            if (a == null)
            {
                throw new System.NotImplementedException();
            }

            return false;
        }

        public bool IsArgumentTrue(KeyDictionary keyDictionary)
        {
            for (var i = 0; i < Keys.Length; i++)
            {
                var key = Keys[i];

                if(!keyDictionary[key])
                {
                    return false;
                }
            }

            return true;
        }

        protected bool Equals(KeyCombination other)
        {
            var keys = other.Keys;

            return Keys.All(keys.Contains);
        }

        public override bool Equals(object obj)
        {
            return Equals((KeyCombination) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 0;
                for (int i = 0; i < Keys.Length; i++)
                {
                    var key = Keys[i];
                    hash = hash ^ key.GetHashCode();
                }

                return hash;    
            }
        }

        public static KeyCombination Esc = new KeyCombination(Key.Escape);
        public static KeyCombination LeftAlt = new KeyCombination(Key.AltLeft);
        public static KeyCombination Enter = new KeyCombination(Key.Enter);
        public static KeyCombination O = new KeyCombination(Key.O);
        public static KeyCombination P = new KeyCombination(Key.P);
    }
}