using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Program
    {
        private int _program;

        public void Create()
        {
            _program = GL.CreateProgram();
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(_program, shader.ShaderId);
        }

        public void DetachShader(Shader shader)
        {
            GL.DetachShader(_program, shader.ShaderId);
        }

        public void Link()
        {
            GL.LinkProgram(_program);
        }

        public void Use()
        {
            GL.UseProgram(_program);
        }

        public void Unbind()
        {
            GL.UseProgram(0);    
        }

        public void Delete()
        {
            GL.DeleteProgram(_program);
        }
    }
}
