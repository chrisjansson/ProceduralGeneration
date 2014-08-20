using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class RenderableMesh
    {
        private readonly VertexBufferObject<Vertex3V3N> _vertexBuffer;
        private readonly VertexBufferObject<uint> _elementBuffer;
        private readonly VertexArrayObject _vertexArrayObject;
        public readonly int Faces;

        public RenderableMesh(VertexBufferObject<Vertex3V3N> vertexBuffer, VertexBufferObject<uint> elementBuffer, VertexArrayObject vertexArrayObject, int length)
        {
            Faces = length;
            _vertexArrayObject = vertexArrayObject;
            _elementBuffer = elementBuffer;
            _vertexBuffer = vertexBuffer;
        }

        public VertexArrayObject VertexArrayObject
        {
            get { return _vertexArrayObject; }
        }

        public void Delete()
        {
            _elementBuffer.Delete();
            _vertexBuffer.Delete();
            _vertexArrayObject.Delete();
        }

        public void Update(Mesh3V3N mesh)
        {
            _vertexBuffer.Bind();
            _vertexBuffer.Data(mesh.Vertices, BufferUsageHint.StreamDraw);
            _vertexBuffer.Unbind();
        }
    }
}