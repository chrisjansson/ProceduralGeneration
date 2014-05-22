using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class SimpleMaterial : IBindable
    {
        private IShader _vertexShader;
        private IShader _fragmentShader;
        private IProgram Program { get; set; }
        private readonly OpenGlResourceFactory _openGlResourceFactory;

        public Uniform<Matrix4> ProjectionMatrix { get; private set; }
        public Uniform<Matrix4> ViewMatrix { get; private set; }
        public Uniform<Matrix4> ModelMatrix { get; private set; }
        public Uniform<Vector4> Color { get; private set; }
        public Uniform<Vector3> LightDirection { get; private set; }

        public SimpleMaterial()
        {
            _openGlResourceFactory = new OpenGlResourceFactory();
        }

        public void Create()
        {
            _vertexShader = _openGlResourceFactory.CreateShader(ShaderType.VertexShader);
            _vertexShader.SetSource(VertexShaderSource);
            _vertexShader.Compile();

            _fragmentShader = _openGlResourceFactory.CreateShader(ShaderType.FragmentShader);
            _fragmentShader.SetSource(FragmentShaderSource);
            _fragmentShader.Compile();

            Program = _openGlResourceFactory.CreateProgram();
            Program.Create();
            Program.AttachShader(_vertexShader);
            Program.AttachShader(_fragmentShader);
            Program.Link();

            ProjectionMatrix = Program.GetUniform<Matrix4>("ProjectionMatrix");
            ViewMatrix = Program.GetUniform<Matrix4>("ViewMatrix");
            ModelMatrix = Program.GetUniform<Matrix4>("ModelMatrix");
            Color = Program.GetUniform<Vector4>("Color");
            LightDirection = Program.GetUniform<Vector3>("LightPosition");
        }

        public void Delete()
        {
            Program.DetachShader(_vertexShader);
            Program.DetachShader(_fragmentShader);
            Program.Delete();

            _vertexShader.Delete();
            _fragmentShader.Delete();
        }

        public void Bind()
        {
            Program.Use();
        }

        public void Unbind()
        {
            Program.Unbind();
        }

        private const string VertexShaderSource = @"
#version 330

layout(location = 0)in vec4 position;
layout(location = 1)in vec3 normal;

uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;
uniform vec4 Color;
uniform vec3 LightPosition;

out VertexData
{
    vec4 color;
} vertex;

void main()
{
    gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * position;
    //vec3 dirToLight = normalize((ViewMatrix * vec4(LightPosition, 1)).xyz - (ViewMatrix * ModelMatrix * position).xyz);
    vec3 dirToLight = normalize((ViewMatrix * vec4(0.5, 0.2, 0, 0)).xyz);
    vec3 normCamSpace = (ViewMatrix * ModelMatrix * vec4(normal.x, normal.y, normal.z, 0)).xyz;
    float incidence = dot(normCamSpace, dirToLight);
    incidence = clamp(incidence, 0, 1);
    vertex.color = Color * incidence + vec4(0.2, 0.2, 0.2, 1.0);
}
";

        private const string FragmentShaderSource = @"
#version 330

out vec4 fragColor;

in VertexData
{
    vec4 color;
} vertex;

void main()
{
    fragColor = vertex.color;
}
";
    }
}