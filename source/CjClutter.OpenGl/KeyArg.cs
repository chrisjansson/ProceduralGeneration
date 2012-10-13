using OpenTK.Input;
using System.Linq;

namespace CjClutter.OpenGl
{
    public class KeyArg
    {
        public KeyArg(params Key[] keys)
        {
            Keys = keys;
        }

        public Key[] Keys { get; private set; }

        public static KeyArg operator &(KeyArg a, KeyArg b)
        {
            var left = a.Keys;
            var right = b.Keys;

            var combinedKeys = left.Union(right)
                .Distinct()
                .ToArray();

            return new KeyArg(combinedKeys);
        }

        public static bool operator false(KeyArg a)
        {
            if(a == null)
            {
                throw new System.NotImplementedException();
            }

            return false;
        }

        public static bool operator true(KeyArg a)
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
    }
}