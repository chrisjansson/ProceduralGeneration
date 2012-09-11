using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Program
    {
        public int ProgramId { get; private set; }

        public void Create()
        {
            ProgramId = GL.CreateProgram();
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(ProgramId, shader.ShaderId);
        }

        public void DetachShader(Shader shader)
        {
            GL.DetachShader(ProgramId, shader.ShaderId);
        }

        public void Link()
        {
            GL.LinkProgram(ProgramId);
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public void Unbind()
        {
            GL.UseProgram(0);    
        }

        public void Delete()
        {
            GL.DeleteProgram(ProgramId);
        }
    }
}
