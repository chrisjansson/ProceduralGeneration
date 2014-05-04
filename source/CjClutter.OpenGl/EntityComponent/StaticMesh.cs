using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class StaticMesh : IEntityComponent
    {
        public Mesh3V3N Mesh { get; private set; }
        public bool IsDirty { get; set; }
        public Vector4 Color { get; set; }
        public Matrix4 ModelMatrix { get; set; }

        public void Update(Mesh3V3N mesh)
        {
            Mesh = mesh;
            IsDirty = true;
        }
    }
}