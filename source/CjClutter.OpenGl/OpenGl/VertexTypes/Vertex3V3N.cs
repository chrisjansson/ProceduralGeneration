using System.Runtime.InteropServices;
using OpenTK;

namespace CjClutter.OpenGl.OpenGl.VertexTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3V3N : IBufferDataType
    {
        public Vector3 Position;
        public Vector3 Normal;

        public int PositionOffset
        {
            get { return 0; }
        }

        public int NormalOffset
        {
            get { return 12; }
        }

        public int SizeInBytes
        {
            get { return 24; }
        }
    }
}
