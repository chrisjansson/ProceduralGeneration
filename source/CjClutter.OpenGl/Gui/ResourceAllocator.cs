using System;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace CjClutter.OpenGl.Gui
{
    public interface IResourceAllocator
    {
        RenderableMesh AllocateResourceFor(Mesh3V3N mesh);
    }

    public class ResourceAllocator : IResourceAllocator
    {
        private readonly IOpenGlResourceFactory _resourceFactory;

        public ResourceAllocator(IOpenGlResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public RenderableMesh AllocateResourceFor(Mesh3V3N mesh)
        {
            var vertexBuffer = CreateVertexBuffer(mesh);
            var elementBuffer = CreateElementBuffer(mesh);
            //var vertexArrayObject = CreateAndSetupVertexArrayObject(vertexBuffer, elementBuffer);

            return new RenderableMesh(vertexBuffer, elementBuffer, null, mesh.Faces.Length, this);
        }

        public VertexArrayObject CreateAndSetupVertexArrayObject(VertexBufferObject<Vertex3V3N> vertexBuffer, VertexBufferObject<uint> elementBuffer)
        {
            var vertexArrayObject = _resourceFactory.CreateVertexArrayObject();

            vertexArrayObject.Bind();
            vertexBuffer.Bind();
            elementBuffer.Bind();

            var dummy = new Vertex3V3N();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, dummy.SizeInBytes, dummy.PositionOffset);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, dummy.SizeInBytes, dummy.NormalOffset);
            GL.EnableVertexAttribArray(1);

            vertexArrayObject.Unbind();
            vertexBuffer.Unbind();
            elementBuffer.Unbind();

            return vertexArrayObject;
        }

        private VertexBufferObject<uint> CreateElementBuffer(Mesh3V3N mesh)
        {
            //Todo pick index type depending on mesh size
            if (mesh.Vertices.LongLength > uint.MaxValue) throw new NotImplementedException();

            var indices = mesh.Faces.SelectMany(x => new[] { (uint)x.V0, (uint)x.V1, (uint)x.V2 }).ToArray();
            var elementBuffer = _resourceFactory.CreateVertexBufferObject<uint>(BufferTarget.ElementArrayBuffer, sizeof(uint));
            elementBuffer.Bind();

            elementBuffer.Data(indices);
            elementBuffer.Unbind();

            return elementBuffer;
        }

        private VertexBufferObject<Vertex3V3N> CreateVertexBuffer(Mesh3V3N mesh)
        {
            var vertexBuffer = _resourceFactory.CreateVertexBufferObject<Vertex3V3N>(BufferTarget.ArrayBuffer);

            vertexBuffer.Bind();
            vertexBuffer.Data(mesh.Vertices);
            vertexBuffer.Unbind();

            return vertexBuffer;
        }
    }
}