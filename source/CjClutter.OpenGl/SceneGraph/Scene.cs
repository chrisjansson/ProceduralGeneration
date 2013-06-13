using System;
using System.Collections.Generic;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Scene
    {
        private Mesh _mesh;

        public Scene()
        {
            Meshes = new List<Mesh>();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public List<Mesh> Meshes { get; set; }

        public void Load()
        {
            _mesh = new TerrainGenerator().GenerateMesh();
            Meshes.Add(_mesh);
        }

        public void Update(double elapsedTime)
        {
            var blue = (Math.Sin(elapsedTime) + 1) / 4;
            var color = new Vector4(0.5f, 0.5f, (float)blue + 0.5f, 0.0f);
            _mesh.Color = color;
        }

        public void Unload()
        {
            //_meshResources.RenderProgram.Delete();
            //_meshResources.VerticesVbo.Delete();
        }

    }
}
