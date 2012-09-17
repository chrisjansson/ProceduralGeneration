using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class VertexArrayObject : IBindable
    {
        private int _array;

        public void Create()
        {
            GL.GenVertexArrays(1, out _array);
        }

        public void Bind()
        {
            GL.BindVertexArray(_array);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
