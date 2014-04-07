using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class StaticMesh : IEntityComponent
    {
        public Mesh3V3N Mesh { get; set; }
        public Vector4 Color { get; set; }
        public Matrix4 ModelMatrix { get; set; }
    }
}