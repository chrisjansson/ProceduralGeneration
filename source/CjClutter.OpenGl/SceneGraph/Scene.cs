using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.OpenTk;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Mesh
    {
        public Mesh()
        {
            Vertices = new List<Vertex3V>();
            Faces = new List<Face3>();
        }

        public IList<Vertex3V> Vertices { get; private set; }
        public IList<Face3> Faces { get; private set; }
    }

    public struct Face3
    {
        public ushort V0;
        public ushort V1;
        public ushort V2;
    }

    public class Scene
    {
        private readonly SimplexNoise _noiseGenerator;
        private VertexBufferObject<Vertex3V> _vertexBufferObject;

        private VertexArrayObject _vertexArrayObject;
        private SimpleRenderProgram _simpleRenderProgram;
        private readonly OpenGlResourceFactory _openGlResourceFactory;

        private const int TerrainWidth = 32;
        private const int TerrainHeight = 32;
        private const int NumberOfTriangles = (TerrainWidth - 1) * (TerrainHeight - 1) * 2;

        public Scene()
        {
            _noiseGenerator = new SimplexNoise();
            _openGlResourceFactory = new OpenGlResourceFactory();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public void Load()
        {
            var mesh = new Mesh();

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = i / (double)TerrainWidth * 2;
                    var yin = j / (double)TerrainHeight * 2;
                    var y = 0.2 * _noiseGenerator.Noise(xin, yin);
                    var x = -0.5 + ScaleTo(i, TerrainWidth);
                    var z = -0.5 + ScaleTo(j, TerrainHeight);

                    var position = new Vector3((float)x, (float)y, (float)z);
                    var vertex = new Vertex3V { Position = position };
                    mesh.Vertices.Add(vertex);
                }
            }

            for (var i = 0; i < TerrainWidth - 1; i++)
            {
                for (var j = 0; j < TerrainHeight - 1; j++)
                {
                    var v0 = i * TerrainHeight + j;
                    var v1 = (i + 1) * TerrainHeight + j;
                    var v2 = (i + 1) * TerrainHeight + j + 1;
                    var v3 = i * TerrainHeight + j + 1;

                    var f0 = new Face3 { V0 = (ushort) v0, V1 = (ushort) v1, V2 = (ushort) v2 };
                    var f1 = new Face3 { V0 = (ushort) v0, V1 = (ushort) v2, V2 = (ushort) v3 };

                    mesh.Faces.Add(f0);
                    mesh.Faces.Add(f1);
                }
            }

            _vertexBufferObject = _openGlResourceFactory.CreateVertexBufferObject<Vertex3V>(BufferTarget.ArrayBuffer);
            _vertexBufferObject.Bind();
            _vertexBufferObject.Data(mesh.Vertices.ToArray());
            _vertexBufferObject.Unbind();

            var faceIndices = mesh.Faces
                .SelectMany(x => new[] { x.V0, x.V1, x.V2 })
                .ToArray();

            var elementArray = _openGlResourceFactory.CreateVertexBufferObject<ushort>(BufferTarget.ElementArrayBuffer, sizeof(ushort));
            elementArray.Bind();
            elementArray.Data(faceIndices);
            elementArray.Unbind();

            _simpleRenderProgram = new SimpleRenderProgram();
            _simpleRenderProgram.Create();

            _vertexArrayObject = _openGlResourceFactory.CreateVertexArrayObject();
            _vertexArrayObject.Bind();

            _vertexBufferObject.Bind();
            elementArray.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            _vertexArrayObject.Unbind();
            _vertexBufferObject.Unbind();
            elementArray.Unbind();
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

            GL.DrawElements(BeginMode.Triangles, NumberOfTriangles * 3, DrawElementsType.UnsignedShort, 0);
        }

        public void Unload()
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
