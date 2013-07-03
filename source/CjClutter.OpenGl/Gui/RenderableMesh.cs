using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.VertexTypes;

namespace CjClutter.OpenGl.Gui
{
    public class RenderableMesh
    {
        private readonly VertexBufferObject<Vertex3V3N> _vertexBuffer;
        private readonly VertexBufferObject<uint> _elementBuffer;
        private readonly VertexArrayObject _vertexArrayObject;

        public RenderableMesh(VertexBufferObject<Vertex3V3N> vertexBuffer, VertexBufferObject<uint> elementBuffer, VertexArrayObject vertexArrayObject)
        {
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
    }
}