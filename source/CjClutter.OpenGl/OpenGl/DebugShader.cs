using CjClutter.OpenGl.OpenGl.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class DebugShader : IShader
    {
        private readonly IShader _shader;
        private readonly ILogger _logger;

        public DebugShader(IShader shader, ILogger logger)
        {
            _logger = logger;
            _shader = shader;
        }

        public void SetSource(string source)
        {
            _shader.SetSource(source);
        }

        public void Compile()
        {
            _shader.Compile();
            var shaderInfoLog = GL.GetShaderInfoLog(ShaderId);
            _logger.Warn(shaderInfoLog);
        }

        public void Delete()
        {
            _shader.Delete();
        }

        public int ShaderId { get { return _shader.ShaderId; } }
    }
}