using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class SimpleRenderProgram : IBindable
    {
        private IShader _vertexShader;
        private IShader _fragmentShader;
        private IShader _geometryShader;
        private Program Program { get; set; }
        private readonly OpenGlResourceFactory _openGlResourceFactory;

        public GenericUniform<Matrix4> ProjectionMatrix { get; private set; }
        public GenericUniform<Matrix4> ViewMatrix { get; private set; }
        public GenericUniform<Matrix4> ModelMatrix { get; private set; }
        public GenericUniform<Vector4> Color { get; private set; }

        public SimpleRenderProgram()
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

            _geometryShader = _openGlResourceFactory.CreateShader(ShaderType.GeometryShader);
            _geometryShader.SetSource(GeometryShaderSource);
            _geometryShader.Compile();

            Program = _openGlResourceFactory.CreateProgram();
            Program.Create();
            Program.AttachShader(_vertexShader);
            Program.AttachShader(_geometryShader);
            Program.AttachShader(_fragmentShader);
            Program.Link();

            ProjectionMatrix = Program.GetUniform<Matrix4>("ProjectionMatrix");
            ViewMatrix = Program.GetUniform<Matrix4>("ViewMatrix");
            ModelMatrix = Program.GetUniform<Matrix4>("ModelMatrix");
            Color = Program.GetUniform<Vector4>("Color");
        }

        public void Delete()
        {
            Program.DetachShader(_vertexShader);
            Program.DetachShader(_fragmentShader);
            Program.Delete();

            _vertexShader.Delete();
            _fragmentShader.Delete();
        }

        private const string VertexShaderSource = @"
#version 330

layout(location = 0)in vec4 position;

uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

void main()
{
    gl_Position = ProjectionMatrix * ViewMatrix * position;
}
";

        private const string GeometryShaderSource = @"
#version 330

vec2 WIN_SCALE = vec2(800, 600);
noperspective out vec3 dist;

layout(triangles) in;
layout (triangle_strip, max_vertices=3) out;

void main() 
{
    vec2 p0 = WIN_SCALE * gl_in[0].gl_Position.xy/gl_in[0].gl_Position.w;
    vec2 p1 = WIN_SCALE * gl_in[1].gl_Position.xy/gl_in[1].gl_Position.w;
    vec2 p2 = WIN_SCALE * gl_in[2].gl_Position.xy/gl_in[2].gl_Position.w;

    vec2 v0 = p2-p1;
    vec2 v1 = p2-p0;
    vec2 v2 = p1-p0;
    float area = abs(v1.x*v2.y - v1.y * v2.x);

    dist = vec3(area/length(v0),0,0);
    gl_Position = gl_in[0].gl_Position;
    EmitVertex();

    dist = vec3(0,area/length(v1),0);
    gl_Position = gl_in[1].gl_Position;
    EmitVertex();

    dist = vec3(0,0,area/length(v2));
    gl_Position = gl_in[2].gl_Position;
    EmitVertex();

    EndPrimitive();
}
";

        private const string FragmentShaderSource = @"
#version 330

uniform vec4 Color;
out vec4 fragColor;

noperspective in vec3 dist;

void main()
{
    float nearD = min(min(dist[0],dist[1]),dist[2]);
    float edgeIntensity = exp2(-1.0*nearD*nearD);

    fragColor = (edgeIntensity * vec4(0.1,0.1,0.1,1.0)) + ((1.0-edgeIntensity) * Color);
}
";

        public void Bind()
        {
            Program.Use();
        }

        public void Unbind()
        {
            Program.Unbind();
        }
    }
}
