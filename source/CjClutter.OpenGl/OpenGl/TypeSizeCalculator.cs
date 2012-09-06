using System.Runtime.InteropServices;

namespace CjClutter.OpenGl.OpenGl
{
    public class TypeSizeCalculator
    {
        public static int GetSize<T>() where T : struct
        {
            return Marshal.SizeOf(new T());
        }
    }
}