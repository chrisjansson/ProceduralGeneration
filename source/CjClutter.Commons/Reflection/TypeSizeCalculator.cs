using System.Runtime.InteropServices;

namespace CjClutter.Commons.Reflection
{
    public class TypeSizeCalculator
    {
        public static int GetSize<T>() where T : struct
        {
            return Marshal.SizeOf(new T());
        }
    }
}