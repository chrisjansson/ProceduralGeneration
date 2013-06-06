using System;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.OpenTk;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;

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

    public class Scene
    {
        private readonly OpenGlResourceFactory _openGlResourceFactory;
        private MeshResources _meshResources;

        public Scene()
        {
            _openGlResourceFactory = new OpenGlResourceFactory();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public void Load()
        {
            var mesh = new TerrainGenerator().GenerateMesh();
            _meshResources = new MeshResources();
            _meshResources.Mesh = mesh;

            _meshResources.VerticesVbo = _openGlResourceFactory.CreateVertexBufferObject<Vertex3V>(BufferTarget.ArrayBuffer);
            _meshResources.VerticesVbo.Bind();
            _meshResources.VerticesVbo.Data(mesh.Vertices.ToArray());
            _meshResources.VerticesVbo.Unbind();

            var faceIndices = mesh.Faces
                .SelectMany(x => new[] { x.V0, x.V1, x.V2 })
                .ToArray();

            if(faceIndices.Length > ushort.MaxValue) throw new NotSupportedException("Implement selection of index data type to allow for bigger ranges");

            _meshResources.IndexVbo = _openGlResourceFactory.CreateVertexBufferObject<ushort>(BufferTarget.ElementArrayBuffer, sizeof(ushort));
            _meshResources.IndexVbo.Bind();
            _meshResources.IndexVbo.Data(faceIndices);
            _meshResources.IndexVbo.Unbind();

            _meshResources.RenderProgram = new SimpleRenderProgram();
            _meshResources.RenderProgram.Create();

            _meshResources.VertexArrayObject = _openGlResourceFactory.CreateVertexArrayObject();
            _meshResources.VertexArrayObject.Bind();

            _meshResources.VerticesVbo.Bind();
            _meshResources.IndexVbo.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            _meshResources.VertexArrayObject.Unbind();
            _meshResources.VerticesVbo.Unbind();
            _meshResources.IndexVbo.Unbind();
        }

        public void Update(double elapsedTime)
        {
            _meshResources.RenderProgram.Bind();

            var blue = (Math.Sin(elapsedTime) + 1) / 4;
            var color = new Vector4(0.5f, 0.5f, (float)blue + 0.5f, 0.0f);

            _meshResources.RenderProgram.Color.Set(color);

            _meshResources.RenderProgram.Unbind();
        }

        public void Draw()
        {
            RunWithResourcesBound(DrawMesh, _meshResources.VertexArrayObject, _meshResources.RenderProgram);
        }

        private void DrawMesh()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            var projectionMatrix = ProjectionMatrix.ToMatrix4();
            _meshResources.RenderProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = ViewMatrix.ToMatrix4();
            _meshResources.RenderProgram.ViewMatrix.Set(viewMatrix);

            GL.DrawElements(BeginMode.Triangles, _meshResources.Mesh.Faces.Count * 3, DrawElementsType.UnsignedShort, 0);
        }

        public void Unload()
        {
            _meshResources.RenderProgram.Delete();
            _meshResources.VerticesVbo.Delete();
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
    }
}
