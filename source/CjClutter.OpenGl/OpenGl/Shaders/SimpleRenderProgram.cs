﻿using CjClutter.OpenGl.OpenGl.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class SimpleRenderProgram : RenderProgramBase, IBindable
    {
        private Shader _vertexShader;
        private Shader _fragmentShader;
        private Shader _geometryShader;

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

            _geometryShader = new Shader();
            _geometryShader.Create(ShaderType.GeometryShader);
            _geometryShader.SetSource(GeometryShaderSource);
            _geometryShader.Compile();

            Program = new Program();
            Program.Create();
            Program.AttachShader(_vertexShader);
            Program.AttachShader(_geometryShader);
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

layout(triangles) in;
layout (triangle_strip, max_vertices=3) out;

void main() 
{
    for(int i = 0; i < 3; i++)
    {
        gl_Position = gl_in[i].gl_Position;

        EmitVertex();
    }
    EndPrimitive();
}
";

        private const string FragmentShaderSource = @"
#version 330

uniform vec4 Color;
out vec4 fragColor;

void main()
{
    fragColor = Color;
}
";
    }
}
