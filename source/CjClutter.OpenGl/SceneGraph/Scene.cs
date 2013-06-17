using System;
using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Scene
    {
        public Scene()
        {
            Meshes = new List<Mesh>();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public List<Mesh> Meshes { get; set; }

        public void Load()
        {
            var terrainGenerator = new TerrainGenerator();
            var generate = terrainGenerator.Generate();

            Meshes.AddRange(generate);
        }

        public void Update(double elapsedTime)
        {
            //var blue = (Math.Sin(elapsedTime) + 1) / 4;
            //var color = new Vector4(0.5f, 0.5f, (float)blue + 0.5f, 0.0f);
            //foreach (var mesh in Meshes)
            //{
            //    mesh.Color = color;    
            //}
        }

        public void Unload()
        {
            //_meshResources.RenderProgram.Delete();
            //_meshResources.VerticesVbo.Delete();
        }

    }
}
