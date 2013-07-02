using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public interface IOpenGlResourceFactory
    {
        IProgram CreateProgram();
        IShader CreateShader(ShaderType shaderType);
        VertexArrayObject CreateVertexArrayObject();
        VertexBufferObject<T> CreateVertexBufferObject<T>(BufferTarget bufferTarget) where T : struct, IBufferDataType;
        VertexBufferObject<T> CreateVertexBufferObject<T>(BufferTarget bufferTarget, int sizeOfT) where T : struct;
    }
}