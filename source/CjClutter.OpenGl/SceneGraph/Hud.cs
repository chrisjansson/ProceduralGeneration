using System;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Boolean = OpenTK.Graphics.OpenGL.Boolean;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Hud
    {
        private readonly OpenGlResourceFactory _openGlResourceFactory;
        private Program _program;
        private VertexArrayObject _vertexArrayObject;

        public Hud()
        {
            _openGlResourceFactory = new OpenGlResourceFactory();

            UploadGeometry();
            CreateShaders();
        }

        public void SetTexture(byte[] pixels)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1024, 768, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        }

        public void Draw()
        {
            _program.Use();
            _vertexArrayObject.Bind();

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, 0);

            _vertexArrayObject.Unbind();
            _program.Unbind();
        }

        private void UploadGeometry()
        {
            var vertices = new[]
                {
                    -0.5f, 0.5f, 0.0f, 0.0f, //v0
                    0.5f, 0.5f, 1.0f, 0.0f, //v1
                    0.5f, -0.5f, 1.0f, 1.0f, //v2
                    -0.5f, -0.5f, 0.0f, 1.0f, //v3
                };

            var indices = new ushort[]
                {
                    0, 1, 2,
                    0, 2, 3
                };

            var vertexBuffer = _openGlResourceFactory.CreateVertexBufferObject<float>(BufferTarget.ArrayBuffer, sizeof(float));
            vertexBuffer.Bind();
            vertexBuffer.Data(vertices);
            vertexBuffer.Unbind();

            var elementBuffer = _openGlResourceFactory.CreateVertexBufferObject<ushort>(BufferTarget.ElementArrayBuffer,
                                                                                        sizeof(ushort));
            elementBuffer.Bind();
            elementBuffer.Data(indices);
            elementBuffer.Unbind();

            _vertexArrayObject = _openGlResourceFactory.CreateVertexArrayObject();
            _vertexArrayObject.Bind();

            vertexBuffer.Bind();
            elementBuffer.Bind();

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4*sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            _vertexArrayObject.Unbind();

            elementBuffer.Unbind();
            vertexBuffer.Unbind();
        }

        private void CreateShaders()
        {
            var vertexShader = _openGlResourceFactory.CreateShader(ShaderType.VertexShader);
            vertexShader.SetSource(VertexShaderSource);
            vertexShader.Compile();

            var fragmentShader = _openGlResourceFactory.CreateShader(ShaderType.FragmentShader);
            fragmentShader.SetSource(FragmentShaderSource);
            fragmentShader.Compile();

            _program = _openGlResourceFactory.CreateProgram();
            _program.Create();
            _program.AttachShader(vertexShader);
            _program.AttachShader(fragmentShader);
            _program.Link();
        }

        private const string VertexShaderSource =
@"#version 330
layout(location = 0)in vec2 position;
layout(location = 1)in vec2 textureCoordinate;
out vec2 TextureCoordinate;

void main()
{
    gl_Position = vec4(position.x*2, position.y*2, 0, 1);
    TextureCoordinate = textureCoordinate;
}";

        private const string FragmentShaderSource =
    @"#version 330

in vec2 TextureCoordinate;
out vec4 fragColor;

uniform sampler2D texture;

void main()
{
    fragColor = texture(texture, TextureCoordinate);
}";
    }
}