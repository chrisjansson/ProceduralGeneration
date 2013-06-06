using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class TerrainGenerator
    {
        private readonly INoiseGenerator _noiseGenerator;
        private const int TerrainWidth = 32;
        private const int TerrainHeight = 32;

        public TerrainGenerator()
        {
            _noiseGenerator = new SimplexNoise();
        }

        public Mesh GenerateMesh()
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

                    var f0 = new Face3 { V0 = (ushort)v0, V1 = (ushort)v1, V2 = (ushort)v2 };
                    var f1 = new Face3 { V0 = (ushort)v0, V1 = (ushort)v2, V2 = (ushort)v3 };

                    mesh.Faces.Add(f0);
                    mesh.Faces.Add(f1);
                }
            }

            return mesh;
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }
    }
}