using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;
using System.Linq;

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

        public void Reload(FractalBrownianMotionSettings fractalBrownianMotionSettings)
        {
            Meshes.Clear();

            var heightMapStream = File.OpenRead("srtm_ramp2.world.5400x2700.jpg");
            var bitmap = new Bitmap(heightMapStream);

            var heightMap = new float[bitmap.Width * bitmap.Height];
            for (var width = 0; width < bitmap.Width; width++)
            {
                for (var height = 0; height < bitmap.Height; height++)
                {
                    var pixel = bitmap.GetPixel(width, height);
                    var brightness = pixel.GetBrightness();
                    var index = width * bitmap.Height + height;
                    heightMap[index] = brightness;
                }
            }

            var TerrainWidth = bitmap.Width;
            var TerrainHeight = bitmap.Height;
            var mesh = new Mesh();

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = (i / (double)TerrainWidth) * 2;
                    var yin = j / (double)TerrainHeight;
                    int index0 = bitmap.Height * (i) + (j);
                    var y = 0.1 * heightMap[index0];

                    var position = new Vector3((float)xin, (float)y, (float)yin);
                    var vertex = new Vertex3V { Position = position };
                    mesh.Vertices.Add(vertex);
                }
            }

            int verticesInColumn = (TerrainHeight);
            for (var i = 0; i < TerrainWidth-1; i++)
            {
                for (var j = 0; j < TerrainHeight-1; j++)
                {
                    var v0 = i * verticesInColumn + j;
                    var v1 = (i + 1) * verticesInColumn + j;
                    var v2 = (i + 1) * verticesInColumn + j + 1;
                    var v3 = i * verticesInColumn + j + 1;

                    var f0 = new Face3 { V0 = (uint)v0, V1 = (uint)v1, V2 = (uint)v2 };
                    var f1 = new Face3 { V0 = (uint)v0, V1 = (uint)v2, V2 = (uint)v3 };

                    mesh.Faces.Add(f0);
                    mesh.Faces.Add(f1);
                }
            }

            mesh.ModelMatrix = Matrix4.CreateTranslation(-1, 0, (float)(-0.5));
            var colorCycle = new ColorCycle();
            mesh.Color = colorCycle.GetNext();
            Meshes.Add(mesh);

            //var terrainGenerator = new TerrainGenerator(fractalBrownianMotionSettings);
            //var generate = terrainGenerator.Generate();

            //Meshes.AddRange(generate);
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
