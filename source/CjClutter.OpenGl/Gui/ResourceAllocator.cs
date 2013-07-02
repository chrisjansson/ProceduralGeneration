using System;
using CjClutter.Commons.Reflection;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace CjClutter.OpenGl.Gui
{
    public class RenderableMesh
    {
        
    }

    public class ResourceAllocator
    {
        private readonly IOpenGlResourceFactory _resourceFactory;

        public ResourceAllocator(IOpenGlResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        public void AllocateResourceFor(Mesh3V3N mesh)
        {
            var vertexBuffer = FillVertexBuffer(mesh);
            var elementBuffer = FillElementBuffer(mesh);

            var vertexArrayObject = _resourceFactory.CreateVertexArrayObject();

            vertexArrayObject.Bind();
            vertexBuffer.Bind();

            var dummy = new Vertex3V3N();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, dummy.SizeInBytes, dummy.PositionOffset);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, dummy.SizeInBytes, dummy.NormalOffset);
            GL.EnableVertexAttribArray(1);

            vertexBuffer.Unbind();
            vertexArrayObject.Unbind();
        }

        private VertexBufferObject<uint> FillElementBuffer(Mesh3V3N mesh)
        {
            //Todo pick index type depending on mesh size
            if (mesh.Vertices.LongLength > int.MaxValue) throw new NotImplementedException();

            var indices = mesh.Faces.SelectMany(x => new[] {(uint) x.V0, (uint) x.V1, (uint) x.V2}).ToArray();
            var elementBuffer = _resourceFactory.CreateVertexBufferObject<uint>(BufferTarget.ElementArrayBuffer, sizeof (uint));
            elementBuffer.Bind();

            elementBuffer.Data(indices);
            elementBuffer.Unbind();

            return elementBuffer;
        }

        private VertexBufferObject<Vertex3V3N> FillVertexBuffer(Mesh3V3N mesh)
        {
            var vertexBuffer = _resourceFactory.CreateVertexBufferObject<Vertex3V3N>(BufferTarget.ArrayBuffer);

            vertexBuffer.Bind();
            vertexBuffer.Data(mesh.Vertices, BufferUsageHint.StaticDraw);
            vertexBuffer.Unbind();

            return vertexBuffer;
        }
    }
}