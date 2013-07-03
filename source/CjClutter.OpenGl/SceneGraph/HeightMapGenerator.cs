using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class HeightMapGenerator
    {
        private readonly INoiseGenerator _noiseGenerator;
        private const int TerrainWidth = 32;
        private const int TerrainHeight = 32;

        public HeightMapGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }

        public Mesh3V3N GenerateMesh()
        {
            var vertices = new List<Vertex3V3N>();
            for (var i = 0; i <= TerrainWidth; i++)
            {
                for (var j = 0; j <= TerrainHeight; j++)
                {
                    var xin = i/(double) TerrainWidth;
                    var yin = j/(double) TerrainHeight;
                    var y = 0.2 * _noiseGenerator.Noise(xin, yin);

                    var position = new Vector3((float)xin, (float)y, (float)yin);
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
            mesh3V3N.CalculateNormals();
            return mesh3V3N;
        }
    }
}