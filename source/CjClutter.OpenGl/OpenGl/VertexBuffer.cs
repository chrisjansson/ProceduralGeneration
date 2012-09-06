using System;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class VertexBuffer<T> where T : struct, IBufferDataType
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

        public void Data(T[] bufferData, BufferUsageHint usageHint)
        {
            var dataSize = bufferData.Length * _sizeInBytes;
            var intPtr = new IntPtr(dataSize);

            Bind();

            GL.BufferData(BufferTarget.ArrayBuffer, intPtr, bufferData, usageHint);

            Unbind();
        }

        public void Destroy()
        {
            GL.DeleteBuffers(1, ref _vertexBufferObject);
            _vertexBufferObject = 0;
        }
    }
}
