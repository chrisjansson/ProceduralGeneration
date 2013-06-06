using System;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class VertexBufferObject<T> where T : struct
    {
        private int _vertexBufferObject;
        private readonly int _sizeInBytes;
        private readonly BufferTarget _target;

        public VertexBufferObject(BufferTarget target, int sizeOfT)
        {
            _target = target;
            _sizeInBytes = sizeOfT;
        }

        public void Generate()
        {
            GL.GenBuffers(1, out _vertexBufferObject);
        }

        public void Bind()
        {
            GL.BindBuffer(_target, _vertexBufferObject);
        }

        public void Unbind()
        {
            GL.BindBuffer(_target, 0);
        }

        public void Data(T[] bufferData, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
        {
            var dataSize = bufferData.Length * _sizeInBytes;
            var size = new IntPtr(dataSize);

            GL.BufferData(_target, size, bufferData, usageHint);
        }

        public void Delete()
        {
            GL.DeleteBuffers(1, ref _vertexBufferObject);
            _vertexBufferObject = 0;
        }
    }
}
