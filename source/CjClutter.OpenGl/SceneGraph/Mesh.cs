using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using Gwen;
using OpenTK;
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
        public Vector4 Color { get; set; }

        public Matrix4 ModelMatrix { get; set; }
    }

    public class SceneObject
    {
        public Mesh3V3N Mesh { get; set; }
        public Matrix4 ModelMatrix { get; set; }
        public Vector4 Color { get; set; }

        public virtual void Update(double elapsedTime) { }
    }

    public class WaterSceneObject : SceneObject
    {
        private ImprovedPerlinNoise _improvedPerlinNoise;
        private const int TerrainWidth = 200;
        private const int TerrainHeight = 200;

        public WaterSceneObject()
        {
            _improvedPerlinNoise = new ImprovedPerlinNoise();
            ModelMatrix = Matrix4.Identity;
            Color = new Vector4(0.2f, 0.2f, 0.8f, 1.0f);

            var vertices = new List<Vertex3V3N>();
            for (var i = 0; i <= TerrainWidth; i++)
            {
                for (var j = 0; j <= TerrainHeight; j++)
                {
                    var xin = i / (double)TerrainWidth * 10;
                    var yin = j / (double)TerrainHeight * 10;
                    
                    var position = new Vector3((float)xin, 0, (float)yin);
                    var vertex = new Vertex3V3N { Position = position };
                    vertices.Add(vertex);
                }
            }

            var faces = new List<Face3>();
            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    const int verticesInColumn = (TerrainHeight + 1);
                    var v0 = i * verticesInColumn + j;
                    var v1 = (i + 1) * verticesInColumn + j;
                    var v2 = (i + 1) * verticesInColumn + j + 1;
                    var v3 = i * verticesInColumn + j + 1;

                    var f0 = new Face3 { V0 = v0, V1 = v1, V2 = v2 };
                    var f1 = new Face3 { V0 = v0, V1 = v2, V2 = v3 };

                    faces.Add(f0);
                    faces.Add(f1);
                }
            }

            var mesh3V3N = new Mesh3V3N(vertices, faces);
            Mesh = mesh3V3N;
            ModelMatrix = Matrix4.CreateTranslation(-5, 0, -5);
            UpdateHeight(0);
        }

        private void UpdateHeight(double time)
        {
            for (var i = 0; i < Mesh.Vertices.Length; i++)
            {
                var position = Mesh.Vertices[i].Position;
                var height = (float) _improvedPerlinNoise.Noise(position.X, position.Z, time);
                Mesh.Vertices[i] = new Vertex3V3N
                {
                    Position = new Vector3(position.X, height, position.Z)
                };
            }
            Mesh.CalculateNormals();
            Mesh.IsDirty = true;
        }

        public override void Update(double elapsedTime)
        {
            UpdateHeight(elapsedTime / 100);
        }
    }

    public class Mesh3V3N
    {
        private readonly Vertex3V3N[] _vertices;
        private readonly Face3[] _faces;

        public Mesh3V3N(IEnumerable<Vertex3V3N> vertices, IEnumerable<Face3> faces)
        {
            _vertices = vertices.ToArray();
            _faces = faces.ToArray();
            IsDirty = true;
        }

        public Vertex3V3N[] Vertices
        {
            get { return _vertices; }
        }

        public Face3[] Faces
        {
            get { return _faces; }
        }

        public bool IsDirty { get; set; }

        public void CalculateNormals()
        {
            for (var i = 0; i < _faces.Length; i++)
            {
                var face = _faces[i];

                var v0 = _vertices[face.V0].Position;
                var v1 = _vertices[face.V1].Position;
                var v2 = _vertices[face.V2].Position;

                var u = v1 - v0;
                var v = v2 - v0;
                u.Normalize();
                v.Normalize();

                var normal = -Vector3.Cross(u, v);
                normal.Normalize();

                _vertices[face.V0].Normal += normal;
                _vertices[face.V1].Normal += normal;
                _vertices[face.V2].Normal += normal;
            }

            for (var index = 0; index < _vertices.Length; index++)
            {
                _vertices[index].Normal.Normalize();
            }
        }
    }
}