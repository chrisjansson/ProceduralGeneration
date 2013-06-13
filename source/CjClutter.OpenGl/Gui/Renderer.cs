using System;
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
        private ProjectionMode _projectionMode = ProjectionMode.Perspective;
        private Matrix4d _projectionMatrix;
        private MeshResources _resourcesForMesh;

        public void Render(Scene scene, ICamera camera)
        {
            var cameraMatrix = camera.GetCameraMatrix();
            scene.ViewMatrix = cameraMatrix;
            scene.ProjectionMatrix = _projectionMatrix;

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var mesh in scene.Meshes)
            {
                var resources = CreateResourcesForMesh(mesh);
                RunWithResourcesBound(
                    () => DrawMesh(scene, resources), 
                    resources.VertexArrayObject, 
                    resources.RenderProgram);
            }
        }

        private void DrawMesh(Scene scene, MeshResources meshResources)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            var projectionMatrix = scene.ProjectionMatrix.ToMatrix4();
            meshResources.RenderProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = scene.ViewMatrix.ToMatrix4();
            meshResources.RenderProgram.ViewMatrix.Set(viewMatrix);

            meshResources.RenderProgram.Color.Set(meshResources.Mesh.Color);

            GL.DrawElements(BeginMode.Triangles, meshResources.Mesh.Faces.Count * 3, DrawElementsType.UnsignedShort, 0);
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

        private MeshResources CreateResourcesForMesh(Mesh mesh)
        {
            if (_resourcesForMesh != null) return _resourcesForMesh;

            var openGlResourceFactory = new OpenGlResourceFactory();
            _resourcesForMesh = new MeshResources();
            _resourcesForMesh.Mesh = mesh;

            _resourcesForMesh.VerticesVbo = openGlResourceFactory.CreateVertexBufferObject<Vertex3V>(BufferTarget.ArrayBuffer);
            _resourcesForMesh.VerticesVbo.Bind();
            _resourcesForMesh.VerticesVbo.Data(mesh.Vertices.ToArray());
            _resourcesForMesh.VerticesVbo.Unbind();

            var faceIndices = mesh.Faces
                .SelectMany(x => new[] { x.V0, x.V1, x.V2 })
                .ToArray();

            if (faceIndices.Length > ushort.MaxValue) throw new NotSupportedException("Implement selection of index data type to allow for bigger ranges");

            _resourcesForMesh.IndexVbo = openGlResourceFactory.CreateVertexBufferObject<ushort>(BufferTarget.ElementArrayBuffer, sizeof(ushort));
            _resourcesForMesh.IndexVbo.Bind();
            _resourcesForMesh.IndexVbo.Data(faceIndices);
            _resourcesForMesh.IndexVbo.Unbind();

            _resourcesForMesh.RenderProgram = new SimpleRenderProgram();
            _resourcesForMesh.RenderProgram.Create();

            _resourcesForMesh.VertexArrayObject = openGlResourceFactory.CreateVertexArrayObject();
            _resourcesForMesh.VertexArrayObject.Bind();

            _resourcesForMesh.VerticesVbo.Bind();
            _resourcesForMesh.IndexVbo.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            _resourcesForMesh.VertexArrayObject.Unbind();
            _resourcesForMesh.VerticesVbo.Unbind();
            _resourcesForMesh.IndexVbo.Unbind();

            return _resourcesForMesh;
        }

        public void Resize(int width, int height)
        {
            _projectionMatrix = CreateProjectionMatrix(width, height);
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
        }
    }
}