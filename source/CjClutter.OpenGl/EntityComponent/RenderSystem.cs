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
        private NormalDebugProgram _normalDebugProgram;

        public RenderSystem(ICamera camera)
        {
            _camera = camera;
            _resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());
            _simpleRenderProgram = new SimpleRenderProgram();
            _simpleRenderProgram.Create();
            _normalDebugProgram = new NormalDebugProgram();
            _normalDebugProgram.Create();
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            _simpleRenderProgram.Bind();
            _simpleRenderProgram.ProjectionMatrix.Set(_camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleRenderProgram.ViewMatrix.Set(_camera.ComputeCameraMatrix().ToMatrix4());
            //meshResources.RenderProgram.WindowScale.Set(_windowScale);

            foreach (var entity in entityManager.GetEntitiesWithComponent<StaticMesh>())
            {
                var component = entityManager.GetComponent<StaticMesh>(entity);
                var resources = EnsureResources(component);

                resources.VertexArrayObject.Bind();
                
                _simpleRenderProgram.ModelMatrix.Set(component.ModelMatrix);
                _simpleRenderProgram.Color.Set(component.Color);

                GL.DrawElements(BeginMode.Triangles, component.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);

                resources.VertexArrayObject.Unbind();
            }
            _simpleRenderProgram.Unbind();

            _normalDebugProgram.Bind();
            _normalDebugProgram.ProjectionMatrix.Set(_camera.ComputeProjectionMatrix().ToMatrix4());
            _normalDebugProgram.ViewMatrix.Set(_camera.ComputeCameraMatrix().ToMatrix4());
            foreach (var entity in entityManager.GetEntitiesWithComponent<NormalComponent>())
            {
                var component = entityManager.GetComponent<StaticMesh>(entity);
                var resources = EnsureResources(component);

                resources.VertexArrayObject.Bind();

                _normalDebugProgram.ModelMatrix.Set(component.ModelMatrix);

                GL.DrawElements(BeginMode.Triangles, component.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);

                resources.VertexArrayObject.Unbind();

            }
            _normalDebugProgram.Unbind();
        }

        private RenderableMesh EnsureResources(StaticMesh component)
        {
            if (!_allocatedResources.ContainsKey(component))
            {
                _allocatedResources[component] = _resourceAllocator.AllocateResourceFor(component.Mesh);
            }

            return _allocatedResources[component];
        }
    }
}