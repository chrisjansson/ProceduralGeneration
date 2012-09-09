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

        public ShaderInfo GetCompileStatus()
        {
            int status;
            GL.GetShader(ShaderId, ShaderParameter.CompileStatus, out status);

            string message;
            GL.GetShaderInfoLog(ShaderId, out message);

            return new ShaderInfo(status, message);
        }

        public void Delete()
        {
            GL.DeleteShader(ShaderId);
        }
    }
}
