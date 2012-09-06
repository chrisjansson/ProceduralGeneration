using OpenTK;

namespace CjClutter.OpenGl.OpenGl.VertexTypes
{
    struct Vertex3V3N : IBufferDataType
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public int SizeInBytes { get { return 24; } }
    }
}
