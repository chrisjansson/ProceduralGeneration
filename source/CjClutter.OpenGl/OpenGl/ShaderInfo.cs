using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class ShaderInfo
    {
        public ShaderInfo(int status, string shaderInfo)
        {
            Status = (Boolean) status;
            Message = shaderInfo;
        }

        public Boolean Status { get; private set; }
        public string Message { get; private set; }
    }
}