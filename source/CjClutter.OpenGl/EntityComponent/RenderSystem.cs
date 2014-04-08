using System.Collections.Generic;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.EntityComponent
{
    public class RenderSystem : IEntitySystem
    {
        private readonly ResourceAllocator _resourceAllocator;
        private readonly Dictionary<StaticMesh, RenderableMesh> _allocatedResources = new Dictionary<StaticMesh, RenderableMesh>();
        private readonly SimpleRenderProgram _simpleRenderProgram;
        private readonly ICamera _camera;

        public RenderSystem(ICamera camera)
        {
            _camera = camera;
            _resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());
            _simpleRenderProgram = new SimpleRenderProgram();
            _simpleRenderProgram.Create();
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _simpleRenderProgram.Bind();
            _simpleRenderProgram.ProjectionMatrix.Set(_camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleRenderProgram.ViewMatrix.Set(_camera.ComputeCameraMatrix().ToMatrix4());

            foreach (var entity in entityManager.GetEntitiesWithComponent<StaticMesh>())
            {
                var component = entityManager.GetComponent<StaticMesh>(entity);
                if (!_allocatedResources.ContainsKey(component))
                {
                    _allocatedResources[component] = _resourceAllocator.AllocateResourceFor(component.Mesh);
                }

                var resources = _allocatedResources[component];
                resources.VertexArrayObject.Bind();
                

                GL.Enable(EnableCap.DepthTest);
                GL.CullFace(CullFaceMode.Back);
                GL.Enable(EnableCap.CullFace);
                GL.FrontFace(FrontFaceDirection.Cw);

                _simpleRenderProgram.ModelMatrix.Set(component.ModelMatrix);
                _simpleRenderProgram.Color.Set(component.Color);

                //meshResources.RenderProgram.WindowScale.Set(_windowScale);
                //meshResources.RenderProgram.ModelMatrix.Set(sceneObject.ModelMatrix);

                GL.DrawElements(BeginMode.Triangles, component.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);

                resources.VertexArrayObject.Unbind();
            }

            _simpleRenderProgram.Unbind();
        }
    }
}