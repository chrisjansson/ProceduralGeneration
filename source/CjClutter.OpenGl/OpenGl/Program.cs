using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public interface IProgram
    {
        void AttachShader(IShader shader);
        void DetachShader(IShader shader);
        void Link();
        void Unbind();
        void Delete();
        int GetAttributeLocation(string attributeName);
        int GetNumberOfActiveUniforms();
        int ProgramId { get; }
        GenericUniform<T> GetUniform<T>(string uniformName);
        void Create();
        void Use();
    }

    public class Program : IProgram
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

        public void AttachShader(IShader shader)
        {
            _gl.AttachShader(ProgramId, shader.ShaderId);
        }

        public void DetachShader(IShader shader)
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

        public GenericUniform<T> GetUniform<T>(string uniformName)
        {
            var location = GetUniformLocation(uniformName);

            return new GenericUniform<T>(location);
        } 

        private int GetUniformLocation(string uniformName)
        {
            return _gl.GetUniformLocation(ProgramId, uniformName);
        }

        public int GetAttributeLocation(string attributeName)
        {
            return _gl.GetAttribLocation(ProgramId, attributeName);
        }

        public int GetNumberOfActiveUniforms()
        {
            int numberOfActiveUniforms;
            GL.GetProgram(ProgramId, ProgramParameter.ActiveUniforms, out numberOfActiveUniforms);

            return numberOfActiveUniforms;
        }
    }
}
