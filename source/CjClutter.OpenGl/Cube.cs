using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CjClutter.OpenGl
{
    public class Cube
    {
        private readonly SimpleMaterial _simpleMaterial;
        private readonly RenderableMesh _renderable;

        public Cube()
        {
            _simpleMaterial = new SimpleMaterial();
            var resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());
            _simpleMaterial.Create();

            var mesh = MeshCreator.CreateXZGrid(10, 10);
            _renderable = resourceAllocator.AllocateResourceFor(mesh);
            _renderable.CreateVAO();
        }

        public void Render(ICamera camera, Vector3d lightPosition)
        {
            _simpleMaterial.Bind();

            _simpleMaterial.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleMaterial.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());
            _simpleMaterial.Color.Set(new Vector4(0f, 1f, 0f, 1f));
            _simpleMaterial.ModelMatrix.Set(Matrix4.CreateTranslation((Vector3)lightPosition));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Disable(EnableCap.CullFace);

            _renderable.VertexArrayObject.Bind();
            GL.DrawElements(BeginMode.Triangles, _renderable.Faces * 3, DrawElementsType.UnsignedInt, 0);

            _renderable.VertexArrayObject.Unbind();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.CullFace);

            _simpleMaterial.Unbind();
        }
    }
}