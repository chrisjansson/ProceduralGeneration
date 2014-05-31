using System;
using System.Collections.Generic;
using System.Linq;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.EntityComponent
{
    public class RenderSystem : IEntitySystem
    {
        private readonly ResourceAllocator _resourceAllocator;
        private readonly Dictionary<StaticMesh, RenderableMesh> _allocatedResources = new Dictionary<StaticMesh, RenderableMesh>();
        private readonly SimpleMaterial _simpleMaterial;
        private readonly ICamera _camera;
        private readonly NormalDebugProgram _normalDebugProgram;

        public RenderSystem(ICamera camera)
        {
            _camera = camera;
            _resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());
            _simpleMaterial = new SimpleMaterial();
            _simpleMaterial.Create();
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

            _simpleMaterial.Bind();

           
            var light = entityManager.GetEntitiesWithComponent<PositionalLightComponent>().Single();
            var positionalLightComponent = entityManager.GetComponent<PositionalLightComponent>(light);
            _simpleMaterial.LightDirection.Set((Vector3) positionalLightComponent.Position);
            _simpleMaterial.ProjectionMatrix.Set(_camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleMaterial.ViewMatrix.Set(_camera.ComputeCameraMatrix().ToMatrix4());

            foreach (var entity in entityManager.GetEntitiesWithComponent<StaticMesh>())
            {
                var component = entityManager.GetComponent<StaticMesh>(entity);
                if(!component.IsVisible)
                    continue;

                component.IsVisible = false;

                var resources = EnsureResources(component);

                resources.VertexArrayObject.Bind();
                
                _simpleMaterial.ModelMatrix.Set(component.ModelMatrix);
                _simpleMaterial.Color.Set(component.Color);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Disable(EnableCap.CullFace);
                
                GL.DrawElements(BeginMode.Triangles, component.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Enable(EnableCap.CullFace);

                resources.VertexArrayObject.Unbind();
            }
            _simpleMaterial.Unbind();

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
            else
            {
                if (component.IsDirty)
                {
                    var oldResources = _allocatedResources[component];
                    oldResources.Delete();

                    _allocatedResources[component] = _resourceAllocator.AllocateResourceFor(component.Mesh);

                    component.IsDirty = false;
                }
            }

            return _allocatedResources[component];
        }
    }
}