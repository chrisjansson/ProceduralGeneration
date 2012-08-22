using System;
using System.Collections.Generic;

namespace CjClutter.OpenGl.Input
{
    public class DelegateComparer : IEqualityComparer<Delegate>
    {
        readonly ByteArrayComparer _byteArrayComparer = new ByteArrayComparer();

        public bool Equals(Delegate x, Delegate y)
        {
            if(x.Target != y.Target)
            {
                return false;
            }

            var firstMethodBody = x.Method.GetMethodBody().GetILAsByteArray();
            var secondMethodBody = y.Method.GetMethodBody().GetILAsByteArray();

            return _byteArrayComparer.Equals(firstMethodBody, secondMethodBody);            
        }

        public int GetHashCode(Delegate obj)
        {
            return obj.GetHashCode();
        }
    }
}
