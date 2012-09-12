using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.OpenTk;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Scene
    {
        private readonly SimplexNoise _noiseGenerator;
        private VertexBuffer<Vertex3V> _vertexBuffer;

        private const int TerrainWidth = 256;
        private const int TerrainHeight = 256;
        private const int NumberOfTriangles = (TerrainWidth - 1)*(TerrainHeight - 1)*2;

        public Scene()
        {
            _noiseGenerator = new SimplexNoise();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public void OnLoad()
        {
            var heightMap = new Vector3[TerrainWidth, TerrainHeight];

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = i / (double)TerrainWidth * 2;
                    var yin = j / (double)TerrainHeight * 2;
                    var y = 0.2 * _noiseGenerator.Noise(xin, yin);
                    var x = -0.5 + ScaleTo(i, TerrainWidth);
                    var z = -0.5 + ScaleTo(j, TerrainHeight);

                    heightMap[i, j] = new Vector3((float)x, (float)y, (float)z);
                }
            }

            var vertices = new Vertex3V[NumberOfTriangles * 3];
            var vertexIndex = 0;

            for (var i = 0; i < TerrainWidth - 1; i++)
            {
                for (var j = 0; j < TerrainHeight - 1; j++)
                {
                    var v0 = heightMap[i, j];
                    var v1 = heightMap[i + 1, j];
                    var v2 = heightMap[i + 1, j + 1];
                    var v3 = heightMap[i, j + 1];

                    vertices[vertexIndex++] = new Vertex3V { Position = v0 };
                    vertices[vertexIndex++] = new Vertex3V { Position = v1 };
                    vertices[vertexIndex++] = new Vertex3V { Position = v2 };

                    vertices[vertexIndex++] = new Vertex3V { Position = v0 };
                    vertices[vertexIndex++] = new Vertex3V { Position = v2 };
                    vertices[vertexIndex++] = new Vertex3V { Position = v3 };
                }
            }

            _vertexBuffer = new VertexBuffer<Vertex3V>();
            _vertexBuffer.Generate();
            _vertexBuffer.Bind();
            _vertexBuffer.Data(vertices);
            _vertexBuffer.Unbind();

            _simpleRenderProgram = new SimpleRenderProgram();
            _simpleRenderProgram.Create();

            _vertexArrayObject = new VertexArrayObject();
            _vertexArrayObject.Create();
            _vertexArrayObject.Bind();

            _vertexBuffer.Bind();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            _vertexArrayObject.Unbind();
        }

        public void Update(double elapsedTime) { }

        public void Draw()
        {
            _vertexArrayObject.Bind();
            _simpleRenderProgram.Bind();

            var projectionMatrix = ProjectionMatrix.ToMatrix4();
            _simpleRenderProgram.ProjectionMatrix = projectionMatrix;
            _simpleRenderProgram.SetUniform(x => x.ProjectionMatrix);

            var viewMatrix = ViewMatrix.ToMatrix4();
            _simpleRenderProgram.ViewMatrix = viewMatrix;
            _simpleRenderProgram.SetUniform(x => x.ViewMatrix);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawArrays(BeginMode.Triangles, 0, NumberOfTriangles * 3);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            _simpleRenderProgram.Unbind();
            _vertexArrayObject.Unbind();
        }

        public void OnUnload()
        {
            _vertexBuffer.Delete();
            _simpleRenderProgram.Delete();
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }

        private VertexArrayObject _vertexArrayObject;
        private SimpleRenderProgram _simpleRenderProgram;
    }
}
