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

        public Uniform<Matrix3> NormalToWorld3x3 { get; private set; }
        public Uniform<Matrix4> ProjectionMatrix { get; private set; }
        public Uniform<Matrix4> ViewMatrix { get; private set; }
        public Uniform<Matrix4> ModelMatrix { get; private set; }
        public Uniform<Vector4> Color { get; private set; }
        public Uniform<Vector3> LightPosition { get; private set; }
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
            LightPosition = Program.GetUniform<Vector3>("ModelSpaceLightPosition");
            NormalToWorld3x3 = Program.GetUniform<Matrix3>("NormalToWorld3x3");
            LightDirection = Program.GetUniform<Vector3>("LightDirection");
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
uniform vec3 LightDirection;

uniform mat4 ModelMatrix;
uniform mat3 NormalToWorld3x3;

in vec3 modelNormal;
in vec4 modelPosition;
out vec4 fragColor;

void main()
{
    vec3 worldSpaceNormal = normalize(NormalToWorld3x3 * modelNormal);
    vec4 worldSpacePosition = ModelMatrix * modelPosition;
    vec3 dirToLight = normalize(ModelSpaceLightPosition - (worldSpacePosition.xyz));
    float positionalIncidence = clamp(dot(worldSpaceNormal, dirToLight), 0, 1);
    float directionalIncidence = clamp(dot(worldSpaceNormal, LightDirection), 0, 1);
    fragColor = Color * directionalIncidence + Color * positionalIncidence;
}
";
    }
}