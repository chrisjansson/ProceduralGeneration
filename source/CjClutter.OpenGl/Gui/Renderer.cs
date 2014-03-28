using System;
using System.Collections.Generic;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class Renderer
    {
        private readonly Dictionary<SceneObject, MeshResources> _resources = new Dictionary<SceneObject, MeshResources>();
        private readonly ResourceAllocator _resourceAllocator;
        private Vector2 _windowScale;

        public Renderer()
        {
            _resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());
        }

        public void Render(Scene scene, ICamera camera)
        {
            var cameraMatrix = camera.ComputeCameraMatrix();
            scene.ViewMatrix = cameraMatrix;

            var projectionMatrix = camera.ComputeProjectionMatrix(_windowScale.X, _windowScale.Y);
            scene.ProjectionMatrix = projectionMatrix;

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var sceneObject in scene.SceneObjects)
            {
                var resources = GetOrCreateResources(sceneObject);
                if (sceneObject.Mesh.IsDirty)
                {
                    if (resources.RenderableMesh != null)
                    {
                        resources.RenderableMesh.Delete();
                    }

                    resources.RenderableMesh = _resourceAllocator.AllocateResourceFor(sceneObject.Mesh);
                }


                var sceneObjectLocalCopy = sceneObject;
                RunWithResourcesBound(
                    () => DrawMesh(scene, sceneObjectLocalCopy, resources),
                    resources.RenderableMesh.VertexArrayObject,
                    resources.RenderProgram);

                RunWithResourcesBound(
                    () => DrawNormals(scene, sceneObjectLocalCopy, resources),
                    resources.RenderableMesh.VertexArrayObject,
                    resources.NormalDebugProgram);
            }
        }

        private void DrawMesh(Scene scene, SceneObject sceneObject, MeshResources meshResources)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            var projectionMatrix = scene.ProjectionMatrix.ToMatrix4();
            meshResources.RenderProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = scene.ViewMatrix.ToMatrix4();
            meshResources.RenderProgram.ViewMatrix.Set(viewMatrix);

            meshResources.RenderProgram.Color.Set(sceneObject.Color);

            meshResources.RenderProgram.WindowScale.Set(_windowScale);
            meshResources.RenderProgram.ModelMatrix.Set(sceneObject.ModelMatrix);

            GL.DrawElements(BeginMode.Triangles, sceneObject.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);
        }

        private void DrawNormals(Scene scene, SceneObject sceneObject, MeshResources meshResources)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            var projectionMatrix = scene.ProjectionMatrix.ToMatrix4();
            meshResources.NormalDebugProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = scene.ViewMatrix.ToMatrix4();
            meshResources.NormalDebugProgram.ViewMatrix.Set(viewMatrix);

            meshResources.NormalDebugProgram.ModelMatrix.Set(sceneObject.ModelMatrix);

            GL.DrawElements(BeginMode.Triangles, sceneObject.Mesh.Faces.Length * 3, DrawElementsType.UnsignedInt, 0);
        }

        private void RunWithResourcesBound(Action action, params IBindable[] bindables)
        {
            foreach (var bindable in bindables)
            {
                bindable.Bind();
            }

            action();

            foreach (var bindable in bindables)
            {
                bindable.Unbind();
            }
        }

        private MeshResources GetOrCreateResources(SceneObject sceneObject)
        {
            if (_resources.ContainsKey(sceneObject))
            {
                return _resources[sceneObject];
            }

            var simpleRenderProgram = new SimpleRenderProgram();
            simpleRenderProgram.Create();

            var normalDebugProgram = new NormalDebugProgram();
            normalDebugProgram.Create();

            var resources = new MeshResources
                {
                    RenderProgram = simpleRenderProgram,
                    NormalDebugProgram = normalDebugProgram,
                };

            _resources.Add(sceneObject, resources);
            return resources;
        }

        private void ReleaseResources(SceneObject sceneObject)
        {
            var resources = _resources[sceneObject];
            resources.RenderProgram.Delete();
            resources.RenderableMesh.Delete();
        }

        public void Resize(int width, int height)
        {
            _windowScale = new Vector2(width, height);
        }
    }
}