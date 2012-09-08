using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace CjClutter.Commons.Reflection
{
    public class FieldOffsetCalculator
    {
        public static int CalculateFieldOffset<TType, TFieldType>(Expression<Func<TType, TFieldType>> expression)
        {
            var fieldName = PropertyHelper.GetPropertyName(expression);

            var offset = Marshal.OffsetOf(typeof (TType), fieldName);
            return offset.ToInt32();
        }
    }
}
