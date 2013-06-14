using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public interface IShader
    {
        void SetSource(string source);
        void Compile();
        void Delete();
        int ShaderId { get; }
    }

    public class Shader : IShader
    {
        private readonly IGl _gl;

        public Shader(IGl gl)
        {
            _gl = gl;
        }

        public int ShaderId { get; private set; }

        public void Create(ShaderType shaderType)
        {
            ShaderId = _gl.CreateShader(shaderType);
        }

        public void SetSource(string source)
        {
            _gl.ShaderSource(ShaderId, source);
        }

        public void Compile()
        {
            _gl.CompileShader(ShaderId);
        }

        public void Delete()
        {
            _gl.DeleteShader(ShaderId);
        }
    }
}
