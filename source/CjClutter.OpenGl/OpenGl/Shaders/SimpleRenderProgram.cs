using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class SimpleRenderProgram : RenderProgramBase
    {
        private Shader _vertexShader;
        private Shader _fragmentShader;

        public GenericUniform<Matrix4> ProjectionMatrix { get; private set; }
        public GenericUniform<Matrix4> ViewMatrix { get; private set; }
        public GenericUniform<Matrix4> ModelMatrix { get; private set; }
        public GenericUniform<Vector4> Color { get; set; }

        public void Create()
        {
            _vertexShader = new Shader();
            _vertexShader.Create(ShaderType.VertexShader);
            _vertexShader.SetSource(VertexShaderSource);
            _vertexShader.Compile();

            _fragmentShader = new Shader();
            _fragmentShader.Create(ShaderType.FragmentShader);
            _fragmentShader.SetSource(FragmentShaderSource);
            _fragmentShader.Compile();

            Program = new Program();
            Program.Create();
            Program.AttachShader(_vertexShader);
            Program.AttachShader(_fragmentShader);
            Program.Link();

            RegisterUniform(() => ProjectionMatrix, x => ProjectionMatrix = x);
            RegisterUniform(() => ViewMatrix, x => ViewMatrix = x);
            RegisterUniform(() => ModelMatrix, x => ModelMatrix = x);
            RegisterUniform(() => Color, x => Color = x);
        }

        public void Delete()
        {
            Program.DetachShader(_vertexShader);
            Program.DetachShader(_fragmentShader);
            Program.Delete();

            _vertexShader.Delete();
            _fragmentShader.Delete();
        }

        private const string VertexShaderSource = @"#version 330

layout(location = 0)in vec4 position;

uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

void main()
{
    gl_Position = ProjectionMatrix * ViewMatrix * position;
}";

        private const string FragmentShaderSource = @"#version 330

uniform vec4 Color;

out vec4 fragColor;

void main()
{
    fragColor = Color;
}";
    }
}
