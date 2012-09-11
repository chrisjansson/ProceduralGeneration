using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Shader
    {
        public int ShaderId { get; private set; }

        public void Create(ShaderType shaderType)
        {
            ShaderId = GL.CreateShader(shaderType);
        }

        public void SetSource(string source)
        {
            GL.ShaderSource(ShaderId, source);
        }

        public void Compile()
        {
            GL.CompileShader(ShaderId);
        }

        public void Delete()
        {
            GL.DeleteShader(ShaderId);
        }
    }
}
