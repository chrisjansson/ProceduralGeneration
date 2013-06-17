using System.Collections.Generic;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;

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
}