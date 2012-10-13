using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CjClutter.Commons.Reflection
{
    public class PropertyHelper
    {
        public static string GetPropertyName<TType, TRetValue>(Expression<Func<TType, TRetValue>> expression)
        {
            return GetMemberNameFromExpression(expression);
        }

        public static string GetPropertyName<TRetValue>(Expression<Func<TRetValue>> expression)
        {
            return GetMemberNameFromExpression(expression);
        }

        private static string GetMemberNameFromExpression<T>(Expression<T> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member;

            return memberInfo.Name;
        }
    }
}