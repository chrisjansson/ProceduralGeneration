using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;

namespace CjClutter.OpenGl.SceneGraph
{
    public class MeshResources
    {
        public Mesh Mesh { get; set; }
        
        public VertexBufferObject<Vertex3V> VerticesVbo { get; set; }
        public SimpleRenderProgram RenderProgram { get; set; }
        public VertexArrayObject VertexArrayObject { get; set; }
        public VertexBufferObject<ushort> IndexVbo { get; set; }
    }
}