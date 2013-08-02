using System.Collections.Generic;
using CjClutter.OpenGl.OpenGl.VertexTypes;
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
    }

    public class Mesh3V3N
    {
        private readonly Vertex3V3N[] _vertices;
        private readonly Face3[] _faces;

        public Mesh3V3N(IEnumerable<Vertex3V3N> vertices, IEnumerable<Face3> faces)
        {
            _vertices = vertices.ToArray();
            _faces = faces.ToArray();
        }

        public Vertex3V3N[] Vertices
        {
            get { return _vertices; }
        }

        public Face3[] Faces
        {
            get { return _faces; }
        }

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