using System.Runtime.InteropServices;
using OpenTK;

namespace CjClutter.OpenGl.OpenGl.VertexTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3V : IBufferDataType
    {
        public Vector3 Position;

        public int SizeInBytes { get { return 12; } }
    }
}
