using System;
using System.Collections.Generic;
using System.Linq;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.OpenTk;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class Renderer
    {
        private readonly Dictionary<Mesh, MeshResources> _resources = new Dictionary<Mesh, MeshResources>();
        private ProjectionMode _projectionMode = ProjectionMode.Perspective;
        private Matrix4d _projectionMatrix;
        private Vector2 _windowScale;

        public void Render(Scene scene, ICamera camera)
        {
            var cameraMatrix = camera.GetCameraMatrix();
            scene.ViewMatrix = cameraMatrix;
            scene.ProjectionMatrix = _projectionMatrix;

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var mesh in scene.Meshes)
            {
                var resources = GetOrCreateResources(mesh);
                var meshLocalClosure = mesh;
                RunWithResourcesBound(
                    () => DrawMesh(scene, meshLocalClosure, resources), 
                    resources.VertexArrayObject, 
                    resources.RenderProgram);
            }
        }

        private void DrawMesh(Scene scene, Mesh mesh, MeshResources meshResources)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            var projectionMatrix = scene.ProjectionMatrix.ToMatrix4();
            meshResources.RenderProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = scene.ViewMatrix.ToMatrix4();
            meshResources.RenderProgram.ViewMatrix.Set(viewMatrix);

            meshResources.RenderProgram.Color.Set(mesh.Color);

            meshResources.RenderProgram.WindowScale.Set(_windowScale);
            meshResources.RenderProgram.ModelMatrix.Set(mesh.ModelMatrix);

            GL.DrawElements(BeginMode.Triangles, mesh.Faces.Count * 3, DrawElementsType.UnsignedShort, 0);
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

        

        private MeshResources GetOrCreateResources(Mesh mesh)
        {
            if (_resources.ContainsKey(mesh))
            {
                return _resources[mesh];
            }
            
            var openGlResourceFactory = new OpenGlResourceFactory();
            var resourcesForMesh = new MeshResources();
            _resources.Add(mesh, resourcesForMesh);

            resourcesForMesh.VerticesVbo = openGlResourceFactory.CreateVertexBufferObject<Vertex3V>(BufferTarget.ArrayBuffer);
            resourcesForMesh.VerticesVbo.Bind();
            resourcesForMesh.VerticesVbo.Data(mesh.Vertices.ToArray());
            resourcesForMesh.VerticesVbo.Unbind();

            var faceIndices = mesh.Faces
                .SelectMany(x => new[] { x.V0, x.V1, x.V2 })
                .ToArray();

            if (faceIndices.Length > ushort.MaxValue) throw new NotSupportedException("Implement selection of index data type to allow for bigger ranges");

            resourcesForMesh.IndexVbo = openGlResourceFactory.CreateVertexBufferObject<ushort>(BufferTarget.ElementArrayBuffer, sizeof(ushort));
            resourcesForMesh.IndexVbo.Bind();
            resourcesForMesh.IndexVbo.Data(faceIndices);
            resourcesForMesh.IndexVbo.Unbind();

            resourcesForMesh.RenderProgram = new SimpleRenderProgram();
            resourcesForMesh.RenderProgram.Create();

            resourcesForMesh.VertexArrayObject = openGlResourceFactory.CreateVertexArrayObject();
            resourcesForMesh.VertexArrayObject.Bind();

            resourcesForMesh.VerticesVbo.Bind();
            resourcesForMesh.IndexVbo.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            resourcesForMesh.VertexArrayObject.Unbind();
            resourcesForMesh.VerticesVbo.Unbind();
            resourcesForMesh.IndexVbo.Unbind();

            return resourcesForMesh;
        }

        private void ReleaseResources(Mesh mesh)
        {
            var resources = _resources[mesh];
            resources.RenderProgram.Delete();

            resources.VerticesVbo.Delete();
            resources.IndexVbo.Delete();
            resources.VertexArrayObject.Delete();
        }

        public void Resize(int width, int height)
        {
            _projectionMatrix = CreateProjectionMatrix(width, height);
            _windowScale = new Vector2(width, height);
        }

        private Matrix4d CreateProjectionMatrix(int width, int height)
        {
            if (_projectionMode == ProjectionMode.Perspective)
            {
                return Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4, (double)width / height, 1, 100);
            }

            return Matrix4d.CreateOrthographic(2, 2, 1, 100);
        }

        public void SetProjectionMode(ProjectionMode projectionMode)
        {
            _projectionMode = projectionMode;
            _projectionMatrix = CreateProjectionMatrix((int) _windowScale.X, (int) _windowScale.Y);
        }
    }
}