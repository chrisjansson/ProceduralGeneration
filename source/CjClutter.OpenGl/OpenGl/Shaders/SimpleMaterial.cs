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
            LightDirection = Program.GetUniform<Vector3>("ModelSpaceLightPosition");
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

out vec4 modelPosition;
out vec3 modelNormal;

void main()
{
    modelPosition = position;
    modelNormal = normal;
    gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * position; 
}
";

        private const string FragmentShaderSource = @"
#version 330

uniform vec4 Color;
uniform vec3 ModelSpaceLightPosition;

in vec3 modelNormal;
in vec4 modelPosition;
out vec4 fragColor;

void main()
{
    
    vec3 lightToDir = normalize(ModelSpaceLightPosition - (modelPosition.xyz));
    float incidence = dot(normalize(modelNormal), lightToDir);
    incidence = clamp(incidence, 0, 1);
    fragColor = Color * incidence + vec4(0.2, 0.2, 0.2, 1.0);
}
";
    }
}