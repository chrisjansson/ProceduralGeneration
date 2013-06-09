using System;
using System.Runtime.InteropServices;
using System.Threading;
using Awesomium.Core;
using CjClutter.OpenGl.OpenGl;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Hud
    {
        private readonly OpenGlResourceFactory _openGlResourceFactory;
        private Program _program;
        private VertexArrayObject _vertexArrayObject;
        private WebView _webView;
        private int _texture;

        public Hud()
        {
            _openGlResourceFactory = new OpenGlResourceFactory();

            UploadGeometry();
            CreateShaders();
            CreateWebView();
        }

        private void CreateWebView()
        {
            _webView = WebCore.CreateWebView(1024, 768);
            _webView.LoadingFrameComplete += FrameCompleted;

            _webView.IsTransparent = true;
            _texture = GL.GenTexture();
        }


        public void Close()
        {
            _webView.Dispose();
        }

        public void Resize(int width, int height)
        {
            _webView.Resize(width, height);
        }

        private bool _isLoading = false;
        private void FrameCompleted(object sender, FrameEventArgs e)
        {
            var surface = (BitmapSurface)_webView.Surface;
            const int pixelDepth = 4;
            _width = surface.Width;
            _height = surface.Height;
            _bytes = new byte[surface.Width * surface.Height * pixelDepth];

            var handle = GCHandle.Alloc(_bytes, GCHandleType.Pinned);
            var addrOfPinnedObject = handle.AddrOfPinnedObject();
            surface.CopyTo(addrOfPinnedObject, surface.Width * pixelDepth, pixelDepth, true, false);
            handle.Free();
            _isLoading = false;
            _bufferDone = true;
        }

        private double _deadLine;
        private bool _bufferDone;
        private byte[] _bytes;
        private int _width;
        private int _height;

        public void Update(double elapsedTime, double frameTime)
        {
            WebCore.Update();
            
            if (_isLoading)
            {
                return;
            }

            if (!_bufferDone)
            {
                var html = string.Format("<html><body style='margin: 0px;margin:0px;float: left'><div style='background-color:#FF0000;width: 100px;padding: 5px'>{0}ms<br />{1}fps</div></body></html>", frameTime * 1000, 1 / frameTime);
                _webView.LoadHTML(html);
                _isLoading = true;    

                return;
            }


            SetTexture(_width, _height, _bytes);
            _bufferDone = false;
        }

        public void SetTexture(int width, int height, byte[] pixels)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _program.Use();
            _vertexArrayObject.Bind();

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, 0);

            _vertexArrayObject.Unbind();
            _program.Unbind();

            GL.Disable(EnableCap.Blend);
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
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

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

uniform sampler2D textureSampler;

void main()
{
    fragColor = texture(textureSampler, TextureCoordinate);
}";


    }
}