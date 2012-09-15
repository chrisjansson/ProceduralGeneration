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

            RegisterUniform(() => ProjectionMatrix, x => ProjectionMatrix = x);
            RegisterUniform(() => ViewMatrix, x => ViewMatrix = x);
            RegisterUniform(() => ModelMatrix, x => ModelMatrix = x);
        }

        private void RegisterUniform<T>(Expression<Func<GenericUniform<T>>> getter, Action<GenericUniform<T>> setter)
        {
            var uniformName = PropertyHelper.GetPropertyName(getter);
            var uniformLocation = _program.GetUniformLocation(uniformName);

            var uniform = new GenericUniform<T>(uniformLocation);
            setter(uniform);
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
