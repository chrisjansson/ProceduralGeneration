using System.Collections.Generic;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;
using System.Linq;

namespace CjClutter.OpenGl.SceneGraph
{
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

        public Mesh3V3N Transformed(Matrix4 transform)
        {
            return new Mesh3V3N(
                Vertices.Select(x => new Vertex3V3N
                {
                    Position = Vector3.Transform(x.Position, transform),
                    Normal = Vector3.TransformNormal(x.Normal, transform).Normalized()
                }),
                _faces);
        }
    }
}