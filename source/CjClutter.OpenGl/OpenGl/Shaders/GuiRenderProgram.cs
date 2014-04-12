using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class GuiRenderProgram : IBindable
    {
        private IProgram _program;

        public void Create()
        {
            var openGlResourceFactory = new OpenGlResourceFactory();
            var vertexShader = openGlResourceFactory.CreateShader(ShaderType.VertexShader);
            vertexShader.SetSource(VertexShaderSource);
            vertexShader.Compile();

            var fragmentShader = openGlResourceFactory.CreateShader(ShaderType.FragmentShader);
            fragmentShader.SetSource(FragmentShaderSource);
            fragmentShader.Compile();

            _program = openGlResourceFactory.CreateProgram();
            _program.Create();
            _program.AttachShader(vertexShader);
            _program.AttachShader(fragmentShader);
            _program.Link();
        }
        
        private const string VertexShaderSource = @"#version 330
    layout(location = 0)in vec2 position;
    layout(location = 2)in vec2 texcoord;
    out vec2 Texcoord;
    void main() {
       Texcoord = texcoord;
       gl_Position = vec4(position, 0.0, 1.0);
    }";

        private const string FragmentShaderSource = @"#version 330
    in vec2 Texcoord;
    out vec4 outColor;
    uniform sampler2D tex;
    void main() {
       outColor = texture(tex, Texcoord);
    }";
        public void Bind()
        {
            _program.Use();
        }

        public void Unbind()
        {
            _program.Unbind();
        }
    }
}