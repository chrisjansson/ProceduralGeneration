using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Diagnostics
{
    public class ShaderDiagnostics
    {
        public ShaderInfo GetCompileStatus(Shader shader)
        {
            int status;
            GL.GetShader(shader.ShaderId, ShaderParameter.CompileStatus, out status);

            string message;
            GL.GetShaderInfoLog(shader.ShaderId, out message);

            return new ShaderInfo(status, message);
        }
    }
}
