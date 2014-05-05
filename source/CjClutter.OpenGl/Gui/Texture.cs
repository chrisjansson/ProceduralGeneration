using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class Texture
    {
        private VertexArrayObject _vertexArrayObject;
        private GuiRenderProgram _guiRenderProgram;

        public void Create()
        {
            var openGlResourceFactory = new OpenGlResourceFactory();
            _vertexArrayObject = openGlResourceFactory.CreateVertexArrayObject();
            _vertexArrayObject.Create();
            _vertexArrayObject.Bind();

            _guiRenderProgram = new GuiRenderProgram();
            _guiRenderProgram.Create();
            _guiRenderProgram.Bind();

            var vertices = new[]{
                //  Position      Texcoords
                -1f,  1f, 0.0f, 0.0f, // Top-left
                1f,  1f, 1.0f, 0.0f, // Top-right
                1f, -1f, 1.0f, 1.0f, // Bottom-right
                -1f, -1, 0.0f, 1.0f  // Bottom-left
            };

            var vertexBufferObject = new VertexBufferObject<float>(BufferTarget.ArrayBuffer, sizeof(float));
            vertexBufferObject.Generate();
            vertexBufferObject.Bind();
            vertexBufferObject.Data(vertices);

            var bufferObject = new VertexBufferObject<uint>(BufferTarget.ElementArrayBuffer, sizeof(uint));
            bufferObject.Generate();
            bufferObject.Bind();
            bufferObject.Data(new uint[] { 0, 1, 2, 2, 3, 0 });

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            var texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            var color = new[] { 1.0f, 0.0f, 0.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, color);

            _vertexArrayObject.Unbind();
            vertexBufferObject.Unbind();
            bufferObject.Unbind();
        }

        public void Render()
        {
            _guiRenderProgram.Bind();
            _vertexArrayObject.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            _vertexArrayObject.Unbind();
            _guiRenderProgram.Unbind();
        }

        public void Upload(Frame frame)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, frame.Width, frame.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, frame.Buffer);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}