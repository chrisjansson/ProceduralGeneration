using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Program
    {
        private readonly IGl _gl;

        public Program(IGl gl)
        {
            _gl = gl;
        }

        public int ProgramId { get; private set; }

        public void Create()
        {
            ProgramId = _gl.CreateProgram();
        }

        public void AttachShader(Shader shader)
        {
            _gl.AttachShader(ProgramId, shader.ShaderId);
        }

        public void DetachShader(Shader shader)
        {
            _gl.DetachShader(ProgramId, shader.ShaderId);
        }

        public void Link()
        {
            _gl.LinkProgram(ProgramId);
        }

        public void Use()
        {
            _gl.UseProgram(ProgramId);
        }

        public void Unbind()
        {
            _gl.UseProgram(0);
        }

        public void Delete()
        {
            _gl.DeleteProgram(ProgramId);
        }

        public int GetUniformLocation(string uniformName)
        {
            return _gl.GetUniformLocation(ProgramId, uniformName);
        }

        public int GetAttributeLocation(string attributeName)
        {
            return _gl.GetAttribLocation(ProgramId, attributeName);
        }
    }
}
