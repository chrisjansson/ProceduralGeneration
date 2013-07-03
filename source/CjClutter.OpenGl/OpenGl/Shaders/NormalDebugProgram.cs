using System;
using CjClutter.OpenGl.OpenGl.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Boolean = OpenTK.Graphics.OpenGL.Boolean;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class NormalDebugProgram : IBindable
    {
        private readonly OpenGlResourceFactory _openGlResourceFactory;
        private IShader _vertexShader;
        private IShader _fragmentShader;
        private IShader _geometryShader;

        public NormalDebugProgram()
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
        }

        public Uniform<Matrix4> ModelMatrix { get; set; }
        public Uniform<Matrix4> ViewMatrix { get; set; }
        public Uniform<Matrix4> ProjectionMatrix { get; set; }

        protected IProgram Program { get; set; }

        public void Delete()
        {
            
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

out VertexData
{
    vec3 normal;
} vertexData;

void main()
{
    gl_Position = position;
    vertexData.normal = normal;
}
";
        private const string GeometryShaderSource = @"
#version 330

uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

layout (triangles) in;
layout (line_strip, max_vertices=2) out;

in VertexData 
{
    vec3 normal;
} vertexData[];

void main() 
{
    for(int i = 0; i < 3; ++i)
    {
        gl_Position =  ProjectionMatrix * ViewMatrix * ModelMatrix * gl_in[0].gl_Position;
        EmitVertex();

        vec3 normal = vertexData[i].normal;
        gl_Position =  ProjectionMatrix * ViewMatrix * ModelMatrix * (gl_in[0].gl_Position + vec4(normal, 0) * 0.05) ;
        EmitVertex();

        EndPrimitive();    
    }
}";
        private const string FragmentShaderSource = @"#version 330

out vec4 fragColor;

void main()
{
    fragColor = vec4(0.0, 0.0, 1.0, 0.0);
}
";
    }
}