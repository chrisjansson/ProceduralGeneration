using System;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class VertexBufferObject<T> where T : struct, IBufferDataType
    {
        private int _vertexBufferObject;
        private readonly int _sizeInBytes = new T().SizeInBytes;

        public void Generate()
        {
            GL.GenBuffers(1, out _vertexBufferObject);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Data(T[] bufferData, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
        {
            var dataSize = bufferData.Length * _sizeInBytes;
            var size = new IntPtr(dataSize);

            GL.BufferData(BufferTarget.ArrayBuffer, size, bufferData, usageHint);
        }

        public void Delete()
        {
            GL.DeleteBuffers(1, ref _vertexBufferObject);
            _vertexBufferObject = 0;
        }
    }
}
