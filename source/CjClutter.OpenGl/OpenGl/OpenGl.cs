using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public interface IGl
    {
        int CreateShader(ShaderType shaderType);
        void ShaderSource(int shaderId, string source);
        void CompileShader(int shaderId);
        void DeleteShader(int shaderId);
        int CreateProgram();
        void AttachShader(int programId, int shaderId);
        void DetachShader(int programId, int shaderId);
        void LinkProgram(int programId);
        void UseProgram(int programId);
        void DeleteProgram(int programId);
        int GetUniformLocation(int programId, string uniformName);
        int GetAttribLocation(int programId, string attributeName);
    }

    public class OpenGl : IGl
    {
        public int CreateShader(ShaderType shaderType)
        {
            return GL.CreateShader(shaderType);
        }

        public void ShaderSource(int shaderId, string source)
        {
            GL.ShaderSource(shaderId, source);
        }

        public void CompileShader(int shaderId)
        {
            GL.CompileShader(shaderId);
        }

        public void DeleteShader(int shaderId)
        {
            GL.DeleteShader(shaderId);
        }

        public int CreateProgram()
        {
            return GL.CreateProgram();
        }

        public void AttachShader(int programId, int shaderId)
        {
            GL.AttachShader(programId, shaderId);
        }

        public void DetachShader(int programId, int shaderId)
        {
            GL.DetachShader(programId, shaderId);
        }

        public void LinkProgram(int programId)
        {
            GL.LinkProgram(programId);
        }

        public void UseProgram(int programId)
        {
            GL.UseProgram(programId);
        }

        public void DeleteProgram(int programId)
        {
            GL.DeleteProgram(programId);
        }

        public int GetUniformLocation(int programId, string uniformName)
        {
            return GL.GetUniformLocation(programId, uniformName);
        }

        public int GetAttribLocation(int programId, string attributeName)
        {
            return GL.GetAttribLocation(programId, attributeName);
        }
    }
}
