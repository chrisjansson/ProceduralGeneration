using System;
using System.Collections.Generic;

namespace CjClutter.OpenGl.Input
{
    public class ActionComparer : IEqualityComparer<Action>
    {
        readonly ByteArrayComparer _byteArrayComparer = new ByteArrayComparer();

        public bool Equals(Action x, Action y)
        {
            if(x.Target != y.Target)
            {
                return false;
            }

            var firstMethodBody = x.Method.GetMethodBody().GetILAsByteArray();
            var secondMethodBody = y.Method.GetMethodBody().GetILAsByteArray();

            return _byteArrayComparer.Equals(firstMethodBody, secondMethodBody);
        }

        public int GetHashCode(Action obj)
        {
            return obj.GetHashCode();
        }
    }
}
