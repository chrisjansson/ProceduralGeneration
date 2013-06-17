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

        public Mesh GenerateMesh()
        {
            var mesh = new Mesh();

            for (var i = 0; i <= TerrainWidth; i++)
            {
                for (var j = 0; j <= TerrainHeight; j++)
                {
                    var xin = i/(double) TerrainWidth;
                    var yin = j/(double) TerrainHeight;
                    var y = 0.2 * _noiseGenerator.Noise(xin, yin);

                    var position = new Vector3((float)xin, (float)y, (float)yin);
                    var vertex = new Vertex3V { Position = position };
                    mesh.Vertices.Add(vertex);
                }
            }

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    const int verticesInColumn = (TerrainHeight + 1);
                    var v0 = i * verticesInColumn + j;
                    var v1 = (i + 1) * verticesInColumn + j;
                    var v2 = (i + 1) * verticesInColumn + j + 1;
                    var v3 = i * verticesInColumn + j + 1;

                    var f0 = new Face3 { V0 = (ushort)v0, V1 = (ushort)v1, V2 = (ushort)v2 };
                    var f1 = new Face3 { V0 = (ushort)v0, V1 = (ushort)v2, V2 = (ushort)v3 };

                    mesh.Faces.Add(f0);
                    mesh.Faces.Add(f1);
                }
            }

            return mesh;
        }
    }
}