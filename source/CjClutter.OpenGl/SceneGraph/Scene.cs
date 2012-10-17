using System;
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
        private VertexBufferObject<Vertex3V> _vertexBufferObject;

        private VertexArrayObject _vertexArrayObject;
        private SimpleRenderProgram _simpleRenderProgram;
        private readonly OpenGlResourceFactory _openGlResourceFactory;

        private const int TerrainWidth = 512;
        private const int TerrainHeight = 512;
        private const int NumberOfTriangles = (TerrainWidth - 1) * (TerrainHeight - 1) * 2;

        public Scene()
        {
            _noiseGenerator = new SimplexNoise();
            _openGlResourceFactory = new OpenGlResourceFactory();
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

            _vertexBufferObject = _openGlResourceFactory.CreateVertexBufferObject<Vertex3V>();
            _vertexBufferObject.Bind();
            _vertexBufferObject.Data(vertices);
            _vertexBufferObject.Unbind();

            _simpleRenderProgram = new SimpleRenderProgram();
            _simpleRenderProgram.Create();

            _vertexArrayObject = _openGlResourceFactory.CreateVertexArrayObject();
            _vertexArrayObject.Bind();

            _vertexBufferObject.Bind();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            _vertexArrayObject.Unbind();
        }

        public void Update(double elapsedTime)
        {
            _simpleRenderProgram.Bind();

            var blue = (Math.Sin(elapsedTime) + 1) / 4;
            var color = new Vector4(0.5f, 0.5f, (float)blue + 0.5f, 0.0f);

            _simpleRenderProgram.Color.Set(color);

            _simpleRenderProgram.Unbind();
        }

        public void Draw()
        {
            RunWithResourcesBound(DrawMesh, _vertexArrayObject, _simpleRenderProgram);
        }

        private void DrawMesh()
        {
            GL.Enable(EnableCap.DepthTest);

            var projectionMatrix = ProjectionMatrix.ToMatrix4();
            _simpleRenderProgram.ProjectionMatrix.Set(projectionMatrix);

            var viewMatrix = ViewMatrix.ToMatrix4();
            _simpleRenderProgram.ViewMatrix.Set(viewMatrix);

            GL.DrawArrays(BeginMode.Triangles, 0, NumberOfTriangles * 3);
        }

        public void OnUnload()
        {
            _vertexBufferObject.Delete();
            _simpleRenderProgram.Delete();
        }

        private void RunWithResourcesBound(Action action, params IBindable[] bindables)
        {
            foreach (var bindable in bindables)
            {
                bindable.Bind();
            }

            action();

            foreach (var bindable in bindables)
            {
                bindable.Unbind();
            }
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }
    }
}
