using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class OpenGlResourceFactory
    {
        private readonly OpenGl _openGl;

        public OpenGlResourceFactory()
        {
            _openGl = new OpenGl();
        }

        public Program CreateProgram()
        {
            return new Program(_openGl);
        }

        public Shader CreateShader(ShaderType shaderType)
        {
            var shader = new Shader(_openGl);
            shader.Create(shaderType);

            return shader;
        }

        public VertexArrayObject CreateVertexArrayObject()
        {
            var vertexArrayObject = new VertexArrayObject();
            vertexArrayObject.Create();

            return vertexArrayObject;
        }

        public VertexBufferObject<T> CreateVertexBufferObject<T>() where T : struct, IBufferDataType
        {
            var vertexBufferObject = new VertexBufferObject<T>();
            vertexBufferObject.Generate();

            return vertexBufferObject;
        }
    }
}
