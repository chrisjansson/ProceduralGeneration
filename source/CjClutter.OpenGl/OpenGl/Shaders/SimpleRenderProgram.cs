using System;
using System.Linq.Expressions;
using CjClutter.Commons.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class SimpleRenderProgram
    {
        private Shader _vertexShader;
        private Shader _fragmentShader;
        private Program _program;

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

            _program = new Program();
            _program.Create();
            _program.AttachShader(_vertexShader);
            _program.AttachShader(_fragmentShader);
            _program.Link();

            var projectionLocation = GetUniformLocation(x => x.ProjectionMatrix);
            ProjectionMatrix = new GenericUniform<Matrix4>(projectionLocation);

            var viewLocation = GetUniformLocation(x => x.ViewMatrix);
            ViewMatrix = new GenericUniform<Matrix4>(viewLocation);

            var modelLocation = GetUniformLocation(x => x.ModelMatrix);
            ModelMatrix = new GenericUniform<Matrix4>(modelLocation);
        }

        private int GetUniformLocation<T>(Expression<Func<SimpleRenderProgram, T>> getter)
        {
            var uniformName = PropertyHelper.GetPropertyName(getter);
            var uniformLocation = GL.GetUniformLocation(_program.ProgramId, uniformName);

            return uniformLocation;
        }

        public void Delete()
        {
            _program.DetachShader(_vertexShader);
            _program.DetachShader(_fragmentShader);
            _program.Delete();
        }

        public GenericUniform<Matrix4> ProjectionMatrix { get; private set; }
        public GenericUniform<Matrix4> ViewMatrix { get; private set; }
        public GenericUniform<Matrix4> ModelMatrix { get; private set; }

        public void Bind()
        {
            _program.Use();
        }

        public void Unbind()
        {
            _program.Unbind();
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

out vec4 fragColor;

void main()
{
    fragColor = vec4(1.0, 0.0, 0.0, 1.0);
}";
    }
}
